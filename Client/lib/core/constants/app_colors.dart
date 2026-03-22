import 'package:flutter/material.dart';

/// Provides color palettes for light and dark themes.
class AppColors {
  /// Returns color palette for light theme.
  static const AppColorsData light = AppColorsData(
    primary: Color(0xFF5B5EFE),
    onPrimary: Color(0xFFF1F1F1),
    danger: Color(0xFFFE5B5B),
    onDanger: Color(0xFFF1F1F1),
    warning: Color(0xFFFED85B),
    success: Color(0xFF5BFE76),
    overlay: Color(0xFF9E9E9E),
    background: Color(0xFFF1F1F1),
    onBackground: Color(0xFF333333),
    surface: Color(0xFFFFFFFF),
    onSurface: Color(0xFF333333),
    outline: Color(0xFF333333),
    outlineVariant: Color(0xFF797979),
    keyButtonBackground: Color(0xFFF5F5F5),
    shadow: Color(0xFF000000),
    disabledFill: Color(0xFFDADADA),
    onDisabled: Color(0xFF9A9A9A),
  );

  /// Returns color palette for dark theme.
  static const AppColorsData dark = AppColorsData(
    primary: Color(0xFF5B5EFE),
    onPrimary: Color(0xFFF1F1F1),
    danger: Color(0xFFFE5B5B),
    onDanger: Color(0xFFF1F1F1),
    warning: Color(0xFFFED85B),
    success: Color(0xFF5BFE76),
    overlay: Color(0xFF212121),
    background: Color(0xFF333333),
    onBackground: Color(0xFFF1F1F1),
    surface: Color(0xFF282828),
    onSurface: Color(0xFFF1F1F1),
    outline: Color(0xFFC7C7C7),
    outlineVariant: Color(0xFF202020),
    keyButtonBackground: Color(0xFF3E3E3E),
    shadow: Color(0xFF000000),
    disabledFill: Color(0xFF4A4A4A),
    onDisabled: Color(0xFF828282),
  );

  /// Returns theme-specific color palette based on [Brightness].
  static AppColorsData of(BuildContext context) {
    final bool isLight = Theme.of(context).brightness == Brightness.light;
    return isLight ? light : dark;
  }
}

class AppColorsData {
  final Color primary;
  final Color onPrimary;
  final Color danger;
  final Color onDanger;
  final Color warning;
  final Color success;
  final Color overlay;
  final Color background;
  final Color onBackground;
  final Color surface;
  final Color onSurface;
  final Color outline;
  final Color outlineVariant;
  final Color keyButtonBackground;
  final Color shadow;
  final Color disabledFill;
  final Color onDisabled;

  const AppColorsData({
    required this.primary,
    required this.onPrimary,
    required this.danger,
    required this.onDanger,
    required this.warning,
    required this.success,
    required this.overlay,
    required this.background,
    required this.onBackground,
    required this.surface,
    required this.onSurface,
    required this.outline,
    required this.outlineVariant,
    required this.keyButtonBackground,
    required this.shadow,
    required this.disabledFill,
    required this.onDisabled,
  });
}
