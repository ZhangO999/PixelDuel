#!/usr/bin/env python3
"""Render the sprite data in Art.cs to a PNG so the art can be checked
without opening Unity. Single source of truth stays the C# file."""
import re, sys, io, os
from PIL import Image, ImageDraw

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SRC  = io.open(os.path.join(ROOT, 'Assets/Scripts/Art.cs'), encoding='utf-8').read()

def arrays(src):
    out = {}
    for m in re.finditer(r'string\[\]\s+(\w+)\s*=\s*\{(.*?)\n\s*\};', src, re.S):
        out[m.group(1)] = re.findall(r'"([^"]*)"', m.group(2))
    return out

def palette(src, player):
    pal = {}
    base = re.search(r'Dictionary<char, Color32> Base = .*?\n\s*\};', src, re.S).group(0)
    for c, v in re.findall(r"\{\s*'(.)',\s*Hex\(0x([0-9a-fA-F]+)\)\s*\}", base):
        pal[c] = tuple(int(v[i:i+2], 16) for i in (0, 2, 4)) + (255,)
    body = re.search(r'Palette\(int player\).*?\n    \}', src, re.S).group(0)
    branch = body.split('else')[0] if player == 0 else body.split('else')[1]
    for c, v in re.findall(r"p\['(.)'\]\s*=\s*Hex\(0x([0-9a-fA-F]+)\)", branch):
        pal[c] = tuple(int(v[i:i+2], 16) for i in (0, 2, 4)) + (255,)
    return pal

A = arrays(SRC)

# ---- validate before drawing: ragged rows are the #1 authoring bug ----
problems = []
for name, rows in A.items():
    widths = {len(r) for r in rows}
    if len(widths) > 1:
        for i, r in enumerate(rows):
            problems.append(f'{name} row {i}: width {len(r)} (expected {max(widths, key=lambda w: sum(len(x)==w for x in rows))})')
known = set('.okwsdhgcvpqbmnr1234567')
for name, rows in A.items():
    used = {ch for r in rows for ch in r}
    unknown = used - known
    if unknown:
        problems.append(f'{name}: unknown palette chars {sorted(unknown)}')
if problems:
    print('ART PROBLEMS:')
    for p in problems: print('  ' + p)
else:
    print('art data OK: all rows rectangular, all chars known')

POSES = [('idle', 'LegsIdle'), ('run0', 'LegsRun0'), ('run1', 'LegsRun1'),
         ('run2', 'LegsRun2'), ('run3', 'LegsRun3'),
         ('jump', 'LegsJump'), ('fall', 'LegsFall')]

def draw(rows, pal, scale):
    w, h = max(len(r) for r in rows), len(rows)
    img = Image.new('RGBA', (w * scale, h * scale), (0, 0, 0, 0))
    d = ImageDraw.Draw(img)
    for y, row in enumerate(rows):
        for x, ch in enumerate(row):
            if ch == '.': continue
            d.rectangle([x*scale, y*scale, x*scale+scale-1, y*scale+scale-1],
                        fill=pal.get(ch, (255, 0, 255, 255)))
    return img

SCALE = 8
sheet = Image.new('RGBA', (7 * 16 * SCALE + 8 * 12, 3 * 16 * SCALE + 4 * 12 + 60), (74, 100, 148, 255))
d = ImageDraw.Draw(sheet)
for pi in (0, 1):
    pal = palette(SRC, pi)
    for i, (label, legs) in enumerate(POSES):
        img = draw(A['Torso'] + A[legs], pal, SCALE)
        x = 12 + i * (16 * SCALE + 12)
        y = 12 + pi * (16 * SCALE + 12)
        sheet.alpha_composite(img, (x, y))
        if pi == 1: d.text((x + 4, y + 16 * SCALE + 2), label, fill=(230, 230, 240))
# extras row
pal = palette(SRC, 0)
y = 12 + 2 * (16 * SCALE + 12) + 14
for i, name in enumerate(['GunArt','BulletArt','TileSolid','TileGrass','TileLedge']):
    sheet.alpha_composite(draw(A[name], pal, SCALE), (12 + i * (16 * SCALE + 12), y))
    d.text((12 + i * (16 * SCALE + 12), y + 40), name, fill=(230, 230, 240))

out = os.path.join(ROOT, 'tools/preview.png')
sheet.save(out)
print('wrote', out, sheet.size)
