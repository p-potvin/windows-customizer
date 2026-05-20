from __future__ import annotations

from dataclasses import dataclass
from typing import Dict, List


@dataclass(frozen=True)
class VaultTheme:
    id: str
    name: str
    mode: str
    roles: Dict[str, str]


CORE_COLORS = {
    "base": "#002B36",
    "paper": "#FDF6E3",
    "paper_bright": "#FDFCF7",
    "ink": "#002B36",
    "slate": "#4A5459",
    "muted": "#586E75",
    "gold": "#CC9B21",
    "gold_muted": "#B78C1E",
    "gold_light": "#E5C06A",
    "cyan": "#21B8CC",
    "green": "#4ECC21",
    "burgundy": "#A63D40",
}

REQUIRED_ROLES = {
    "background",
    "surface",
    "surface_elevated",
    "text_primary",
    "text_secondary",
    "accent",
    "accent_hover",
    "border_subtle",
    "focus_ring",
    "success",
    "warning",
    "danger",
}


class VaultThemeManager:
    """Centralized theme definitions and simple export helpers."""

    def __init__(self) -> None:
        self._themes = [
            VaultTheme(
                id="vault-light",
                name="Vault Light",
                mode="light",
                roles={
                    "background": CORE_COLORS["paper"],
                    "surface": CORE_COLORS["paper_bright"],
                    "surface_elevated": "rgba(253, 252, 247, 0.86)",
                    "text_primary": CORE_COLORS["ink"],
                    "text_secondary": CORE_COLORS["muted"],
                    "accent": CORE_COLORS["gold"],
                    "accent_hover": CORE_COLORS["gold_muted"],
                    "border_subtle": "rgba(0, 43, 54, 0.14)",
                    "focus_ring": CORE_COLORS["cyan"],
                    "success": CORE_COLORS["green"],
                    "warning": CORE_COLORS["gold"],
                    "danger": CORE_COLORS["burgundy"],
                },
            ),
            VaultTheme(
                id="vault-dark",
                name="Vault Dark",
                mode="dark",
                roles={
                    "background": CORE_COLORS["base"],
                    "surface": CORE_COLORS["slate"],
                    "surface_elevated": "rgba(74, 84, 89, 0.72)",
                    "text_primary": CORE_COLORS["paper"],
                    "text_secondary": "#D8D0B8",
                    "accent": CORE_COLORS["gold"],
                    "accent_hover": CORE_COLORS["gold_light"],
                    "border_subtle": "rgba(253, 246, 227, 0.18)",
                    "focus_ring": CORE_COLORS["cyan"],
                    "success": CORE_COLORS["green"],
                    "warning": CORE_COLORS["gold"],
                    "danger": CORE_COLORS["burgundy"],
                },
            ),
            VaultTheme(
                id="golden-slate",
                name="Golden Slate",
                mode="dark",
                roles={
                    "background": CORE_COLORS["slate"],
                    "surface": "#3F494E",
                    "surface_elevated": "rgba(74, 84, 89, 0.82)",
                    "text_primary": CORE_COLORS["paper"],
                    "text_secondary": "#D8D0B8",
                    "accent": CORE_COLORS["gold"],
                    "accent_hover": CORE_COLORS["gold_light"],
                    "border_subtle": "rgba(253, 246, 227, 0.18)",
                    "focus_ring": CORE_COLORS["cyan"],
                    "success": CORE_COLORS["green"],
                    "warning": CORE_COLORS["gold"],
                    "danger": CORE_COLORS["burgundy"],
                },
            ),
        ]

        for theme in self._themes:
            self.validate_theme(theme)

    def get_themes(self) -> List[VaultTheme]:
        return list(self._themes)

    def get_theme(self, theme_id: str = "vault-light") -> VaultTheme:
        for theme in self._themes:
            if theme.id == theme_id:
                return theme
        return self._themes[0]

    @staticmethod
    def validate_theme(theme: VaultTheme) -> None:
        missing = REQUIRED_ROLES.difference(theme.roles)
        if missing:
            missing_list = ", ".join(sorted(missing))
            raise ValueError(f"Theme {theme.id} is missing roles: {missing_list}")

    @staticmethod
    def get_glass_rgba(hex_color: str, alpha: float) -> str:
        value = hex_color.lstrip("#")
        if len(value) != 6:
            raise ValueError(f"Expected #RRGGBB color, got {hex_color}")

        red, green, blue = tuple(int(value[index:index + 2], 16) for index in (0, 2, 4))
        return f"rgba({red}, {green}, {blue}, {alpha})"
