"""Build a reproducible icon TTF from a permanent manifest and outline SVGs.

Writes only to --output (a disposable staging directory). Unity commits the
results after successfully generating and validating the TMP atlas.
"""
import argparse
import json
import math
from pathlib import Path
import re
import shutil
import xml.etree.ElementTree as ET

from fontTools.fontBuilder import FontBuilder
from fontTools.pens.boundsPen import BoundsPen
from fontTools.pens.cu2quPen import Cu2QuPen
from fontTools.pens.transformPen import TransformPen
from fontTools.pens.ttGlyphPen import TTGlyphPen
from fontTools.svgLib.path import SVGPath
from fontTools.ttLib import TTFont


class FilledGlyphPen(TTGlyphPen):
    def endPath(self):
        # SVG fills implicitly close open contours. Font contours must close too.
        self.closePath()


def glyph_from_svg(path):
    if path.stat().st_size > 1024 * 1024:
        raise ValueError("SVG is too large (maximum 1 MB)")
    data = path.read_text(encoding="utf-8")
    if "<!DOCTYPE" in data or "<!ENTITY" in data:
        raise ValueError("SVG entities and document types are unsupported")
    root = ET.fromstring(data)
    if root.tag.split("}")[-1] != "svg":
        raise ValueError("Expected an SVG document")
    viewbox = [float(v) for v in re.split(r"[\s,]+", root.get("viewBox", "").strip()) if v]
    if len(viewbox) != 4 or not all(math.isfinite(v) for v in viewbox) or min(viewbox[2:]) <= 0:
        raise ValueError("SVG needs a finite, positive viewBox")
    paths = 0
    for element in root.iter():
        tag = element.tag.split("}")[-1]
        if tag not in {"svg", "g", "path", "title", "desc"}:
            raise ValueError(f"Unsupported SVG element <{tag}>; convert shapes and strokes to filled paths")
        for key, value in element.attrib.items():
            key = key.split("}")[-1]
            if key in {"style", "class", "transform", "clip-path", "mask", "filter", "href", "display", "visibility", "vector-effect"}:
                raise ValueError(f"Unsupported SVG attribute {key}; flatten the SVG to filled paths")
            if key in {"opacity", "fill-opacity", "stroke-opacity"} and value != "1":
                raise ValueError("Transparency is unsupported in monochrome fonts")
            if key == "stroke" and value != "none":
                raise ValueError("Convert SVG strokes to filled outlines first")
            if key == "fill" and (value == "none" or "url(" in value):
                raise ValueError("Only solid filled outlines are supported")
            if key == "fill-rule" and value != "nonzero":
                raise ValueError("Convert even-odd fills to nonzero outlines first")
        if tag == "path":
            paths += 1
            d = element.get("d", "").strip()
            if not d:
                raise ValueError("SVG contains an empty path")
    if not paths:
        raise ValueError("SVG contains no paths")
    # Preserve the viewBox's padding and aspect ratio on a 1000-unit square.
    x, y, width, height = viewbox
    scale = 1000 / max(width, height)
    transform = (scale, 0, 0, -scale,
                 (1000 - width * scale) / 2 - x * scale,
                 800 - (1000 - height * scale) / 2 + y * scale)
    pen = FilledGlyphPen(None)
    SVGPath.fromstring(data).draw(TransformPen(Cu2QuPen(pen, 0.5, reverse_direction=True), transform))
    glyph = pen.glyph()
    bounds = BoundsPen(None)
    glyph.draw(bounds, None)
    if bounds.bounds is None or not all(math.isfinite(v) for v in bounds.bounds):
        raise ValueError("SVG has no visible geometry")
    left, bottom, right, top = bounds.bounds
    if right <= left or top <= bottom or left < -1 or right > 1001 or bottom < -201 or top > 801:
        raise ValueError("SVG geometry must fit inside its viewBox")
    return glyph, round(left)


def build(source, output, svg=None, name=None, reserved=()):
    source, output = Path(source), Path(output)
    if source.resolve() == output.resolve():
        raise ValueError("Output must be a separate staging directory")
    manifest_path = source / "icons.json"
    manifest = json.loads(manifest_path.read_text()) if manifest_path.exists() else {"version": 1, "icons": []}
    if manifest.get("version") != 1:
        raise ValueError("Unsupported icon manifest version")
    icons = manifest["icons"]
    names, codes = set(), set()
    for entry in icons:
        if not re.fullmatch(r"[a-z][a-z0-9_]*", entry["name"]):
            raise ValueError("Invalid icon name in manifest")
        if entry["name"] in names or entry["codepoint"] in codes:
            raise ValueError("Duplicate icon name or codepoint")
        if not 0xE000 <= entry["codepoint"] <= 0xF8FF:
            raise ValueError("Icon codepoints must be in the BMP private-use range")
        if entry["codepoint"] in reserved:
            raise ValueError(f"Codepoint collision for {entry['name']}; resolve the conflicting font without renumbering icons")
        if entry["svg"] != f"Sources/{entry['name']}.svg":
            raise ValueError("Unexpected SVG path in manifest")
        names.add(entry["name"])
        codes.add(entry["codepoint"])
    if svg:
        if not name or not re.fullmatch(r"[a-z][a-z0-9_]*", name):
            raise ValueError("Use a lowercase name starting with a letter, containing letters, digits or underscores")
        if name in names:
            raise ValueError(f"Icon '{name}' already exists; edit its saved SVG and use Rebuild SVG Font")
        code = next((c for c in range(0xE000, 0xF900) if c not in codes and c not in reserved), None)
        if code is None:
            raise ValueError("No unused private-use codepoints remain")
        icons.append({"name": name, "codepoint": code, "svg": f"Sources/{name}.svg"})
    if not icons:
        raise ValueError("No icons to build")
    glyphs = {".notdef": TTGlyphPen(None).glyph()}
    metrics = {".notdef": (1000, 0)}
    inputs = {}
    for entry in icons:
        path = Path(svg) if svg and entry["name"] == name else source / entry["svg"]
        glyphs[entry["name"]], left = glyph_from_svg(path)
        metrics[entry["name"]] = (1000, left)
        inputs[entry["svg"]] = path
    builder = FontBuilder(1000, isTTF=True)
    builder.setupGlyphOrder(list(glyphs))
    builder.setupCharacterMap({entry["codepoint"]: entry["name"] for entry in icons})
    builder.setupGlyf(glyphs)
    builder.setupHorizontalMetrics(metrics)
    builder.setupHorizontalHeader(ascent=800, descent=-200)
    builder.setupNameTable({"familyName": "Custom Icons", "styleName": "Regular",
                            "uniqueFontIdentifier": "CustomIcons-Regular-1", "fullName": "Custom Icons Regular",
                            "psName": "CustomIcons-Regular", "version": "Version 1.000"})
    builder.setupOS2(sTypoAscender=800, sTypoDescender=-200, usWinAscent=800, usWinDescent=200)
    builder.setupPost()
    builder.setupMaxp()
    builder.font["head"].created = builder.font["head"].modified = 2082844800
    builder.font.recalcTimestamp = False
    output.mkdir(parents=True, exist_ok=True)
    builder.save(output / "CustomIcons.ttf")
    with TTFont(output / "CustomIcons.ttf") as font:
        assert font.getBestCmap() == {entry["codepoint"]: entry["name"] for entry in icons}
    for target, path in inputs.items():
        destination = output / target
        destination.parent.mkdir(parents=True, exist_ok=True)
        shutil.copyfile(path, destination)
    (output / "icons.json").write_text(json.dumps(manifest, indent=2) + "\n")
    (output / "CustomIcons.codepoints.txt").write_text("".join(f"{e['name']} {e['codepoint']:04x}\n" for e in icons))
    print(f"Built {len(icons)} icons")


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--source", required=True)
    parser.add_argument("--output", required=True)
    parser.add_argument("--svg")
    parser.add_argument("--name")
    parser.add_argument("--reserved")
    args = parser.parse_args()
    try:
        reserved = set(json.loads(Path(args.reserved).read_text())) if args.reserved else set()
        build(args.source, args.output, args.svg, args.name, reserved)
    except (ValueError, OSError, ET.ParseError) as error:
        parser.exit(1, f"{error}\n")
