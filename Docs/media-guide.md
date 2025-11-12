# Media & GIF Guide

Add gameplay media under `Docs/media/`.

## Recommended Capture Workflow
1. Use Unity Game view at 1080p (or 1280x720 for smaller GIFs).
2. Record with: 
   - OBS Studio (MP4 or MKV) or
   - ScreenToGif (direct GIF capture).
3. Trim & crop to focus on ability interaction.
4. Optimize GIF:
   - Target <4 MB per GIF.
   - 12–20 FPS is sufficient for gameplay previews.
   - Use lossy compression (ScreenToGif or gifski) if needed.

## Naming Convention
- `cast-basic-fire.gif`
- `aoe-mouse-targeting.gif`
- `chain-lightning.gif`
- `heal-overtime.gif`

## Suggested Media
- Basic self-cast ability.
- Mouse AOE placement & activation.
- Projectile firing & impact.
- Chain targeting linking multiple enemies.
- Heal over time ticks visible in logs or UI.
- Cooldown visualization tween in slot.

## Adding to README
Embed with relative paths, e.g.:
```
![Chain Lightning](Docs/media/chain-lightning.gif)
```

## Diagram Source
Store editable diagram sources (Mermaid `.mmd`, PlantUML `.puml`) in `Docs/media/diagram-source/` so contributions can update architecture visuals.

## Attribution
Ensure any third-party media or logos are credited appropriately.

