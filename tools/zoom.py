#!/usr/bin/env python3
"""Zoom one pose, on a grid, for close inspection of the pixel work."""
import sys, os
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from PIL import Image, ImageDraw
import preview as P

legs = sys.argv[1] if len(sys.argv) > 1 else 'LegsIdle'
who  = int(sys.argv[2]) if len(sys.argv) > 2 else 0
rows = P.A['Torso'] + P.A[legs]
pal  = P.palette(P.SRC, who)
S = 26
w, h = max(len(r) for r in rows), len(rows)
img = Image.new('RGBA', (w*S, h*S), (45, 48, 68, 255))
d = ImageDraw.Draw(img)
for y, row in enumerate(rows):
    for x, ch in enumerate(row):
        if ch == '.': continue
        d.rectangle([x*S, y*S, x*S+S-1, y*S+S-1], fill=pal.get(ch, (255, 0, 255, 255)))
for i in range(max(w, h)+1):
    d.line([(i*S, 0), (i*S, h*S)], fill=(255, 255, 255, 30))
    d.line([(0, i*S), (w*S, i*S)], fill=(255, 255, 255, 30))
img.save(os.path.join(P.ROOT, 'tools/zoom.png'))
print('wrote tools/zoom.png', legs, 'player', who)
