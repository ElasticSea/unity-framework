# SVG icon font importer

Use **Window > Icon Explorer > Add SVG** to save a source SVG and generate a custom icon font. Click its preview to copy its Unicode character. Edit a saved SVG and use **Rebuild SVG Font** to update it.

The importer stores sources, permanent codepoint assignments, TTF, codepoint lookup, IconFont pack and static TMP atlas under `Assets/Custom Icons`. It validates the generated font and atlas before updating live assets. Existing font, material and atlas references are retained. The custom TMP font is added as a fallback to existing IconFont packs. Generated content is project data and is not supplied with this tooling.

Input SVGs must use solid filled paths and a finite positive viewBox. Convert strokes and shapes to outlines and flatten transforms first. Unsupported masks, gradients, styles, images and even-odd fills are rejected. Holes must use opposite contour winding. Existing codepoints never change automatically; do not manually renumber or reuse manifest entries.

Python 3 is needed only to add or rebuild icons. The importer creates an isolated environment under `Library/IconFontTools/venv` and installs the pinned requirements on first use, which requires internet access. Set `ICON_FONT_PYTHON` before starting Unity to select a Python executable. Save pending font asset edits before rebuilding.

Run the generator regression tests from this directory with the configured Python environment:

```sh
python -B -m unittest discover -s . -p 'test_*.py'
```

SVG originals and generated font assets should be committed together when publishing icon content. Include source artwork attribution and applicable licenses for each project icon.
