from __future__ import annotations

from theme_manager import VaultTheme, VaultThemeManager


class QtThemeExporter:
    """Generate PySide/PyQt QSS from VaultWares theme roles."""

    def __init__(self) -> None:
        self.manager = VaultThemeManager()

    def get_all_themes(self) -> list[VaultTheme]:
        return self.manager.get_themes()

    def generate_qss(self, theme: VaultTheme) -> str:
        roles = theme.roles
        return f"""
QMainWindow {{
    background-color: {roles["background"]};
    color: {roles["text_primary"]};
}}

QWidget {{
    font-family: "Segoe UI Semilight", "Segoe UI", "Inter";
    color: {roles["text_primary"]};
}}

QLabel {{
    color: {roles["text_primary"]};
    font-size: 12px;
}}

QLineEdit,
QComboBox,
QTextEdit,
QSpinBox {{
    background-color: {roles["surface_elevated"]};
    border: 1px solid {roles["border_subtle"]};
    border-radius: 8px;
    padding: 4px 8px;
    min-height: 28px;
    color: {roles["text_primary"]};
}}

QLineEdit:focus,
QComboBox:focus,
QTextEdit:focus,
QSpinBox:focus {{
    border-color: {roles["focus_ring"]};
}}

QPushButton {{
    background-color: {roles["surface_elevated"]};
    border: 1px solid {roles["border_subtle"]};
    border-radius: 8px;
    padding: 10px 16px;
    color: {roles["text_primary"]};
    font-weight: 500;
}}

QPushButton:hover {{
    border-color: {roles["accent"]};
}}

QPushButton#PrimaryBtn {{
    background-color: {roles["accent"]};
    color: {roles["background"]};
    font-weight: 600;
}}

QPushButton#PrimaryBtn:hover {{
    background-color: {roles["accent_hover"]};
}}

QCheckBox {{
    color: {roles["text_primary"]};
}}

QCheckBox::indicator {{
    width: 18px;
    height: 18px;
    border: 1px solid {roles["border_subtle"]};
    border-radius: 5px;
    background-color: {roles["surface"]};
}}

QCheckBox::indicator:checked {{
    background-color: {roles["accent"]};
    border-color: {roles["accent"]};
}}

QFrame#ConfigPanel,
QFrame#MonitorPanel {{
    background-color: {roles["surface_elevated"]};
    border: 1px solid {roles["border_subtle"]};
    border-radius: 16px;
}}

QProgressBar {{
    background-color: {roles["surface"]};
    border: none;
    border-radius: 6px;
}}

QProgressBar::chunk {{
    background-color: {roles["accent"]};
    border-radius: 6px;
}}
""".strip()
