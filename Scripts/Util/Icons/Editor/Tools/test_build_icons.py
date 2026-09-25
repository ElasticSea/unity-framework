import json
from pathlib import Path
import tempfile
import unittest
import sys

sys.dont_write_bytecode = True

from build_icons import build
from fontTools.ttLib import TTFont


class IconBuildTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.root = Path(self.temp.name)
        self.svg = self.root / "icon.svg"
        self.svg.write_text('<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24"><path d="M2 2H22V22H2Z M8 8V16H16V8Z"/></svg>')

    def tearDown(self):
        self.temp.cleanup()

    def test_add_and_rebuild_preserve_codepoints_and_font_bytes(self):
        first, second, rebuilt = (self.root / p for p in ["first", "second", "rebuilt"])
        build(self.root / "empty", first, self.svg, "pumpkin", {0xE000})
        build(first, second, self.svg, "ghost", {0xE000})
        build(second, rebuilt, reserved={0xE000})
        icons = json.loads((rebuilt / "icons.json").read_text())["icons"]
        self.assertEqual([i["codepoint"] for i in icons], [0xE001, 0xE002])
        self.assertEqual((second / "CustomIcons.ttf").read_bytes(), (rebuilt / "CustomIcons.ttf").read_bytes())
        with TTFont(rebuilt / "CustomIcons.ttf") as font:
            self.assertEqual(font.getBestCmap(), {0xE001: "pumpkin", 0xE002: "ghost"})
            self.assertEqual(font["glyf"]["pumpkin"].numberOfContours, 2)

    def test_invalid_input_never_changes_source_pack(self):
        source = self.root / "source"
        build(self.root / "empty", source, self.svg, "pumpkin")
        original = {p: p.read_bytes() for p in source.rglob("*") if p.is_file()}
        for svg in [
            '<svg viewBox="0 0 24 24"><image href="foo.png"/></svg>',
            '<svg viewBox="0 0 24 24"><path stroke="black" d="M1 1L2 2"/></svg>',
            '<svg viewBox="0 0 24 24"><path transform="scale(2)" d="M1 1H2V2Z"/></svg>',
            '<svg viewBox="0 0 24 24"><path d="M1 1H50V50Z"/></svg>',
        ]:
            self.svg.write_text(svg)
            with self.assertRaises(ValueError):
                build(source, self.root / "failed", self.svg, "bad")
        self.assertEqual(original, {p: p.read_bytes() for p in source.rglob("*") if p.is_file()})

    def test_duplicate_name_and_later_collision_are_rejected(self):
        source = self.root / "source"
        build(self.root / "empty", source, self.svg, "pumpkin")
        with self.assertRaises(ValueError):
            build(source, self.root / "duplicate", self.svg, "pumpkin")
        with self.assertRaises(ValueError):
            build(source, self.root / "collision", reserved={0xE000})

    def test_open_svg_fills_are_closed_for_font_engine(self):
        self.svg.write_text('<svg viewBox="0 0 24 24"><path d="M2 2H22V22H2 M8 8V16H16V8"/></svg>')
        output = self.root / "open"
        build(self.root / "empty", output, self.svg, "open_contours")
        with TTFont(output / "CustomIcons.ttf") as font:
            self.assertEqual(font["glyf"]["open_contours"].numberOfContours, 2)


if __name__ == "__main__":
    unittest.main()
