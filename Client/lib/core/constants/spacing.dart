/// Defines the standard spacing values used throughout the UI layout.
///
/// Values follow a scale from `xxs` (extra-extra small) to `xxl` (extra-extra large)
/// and are used to maintain consistent padding, margin, and gaps in the app.
class Spacing {
  /// Compact sizing, xxs = 4.0
  static const double xxs = 4.0;

  /// UI controls, control + label, xs = 8.0
  static const double xs = 8.0;

  /// Control + header, edge text, sm = 12.0
  static const double sm = 12.0;

  /// List styles, cards, md = 16.0
  static const double md = 16.0;

  /// Content sections, lg = 24.0
  static const double lg = 24.0;

  /// Page padding, xl = 36.0
  static const double xl = 36.0;

  /// Page sections with title, xxl = 48.0
  static const double xxl = 48.0;

  static const KeyGridSpacing keyGrid = KeyGridSpacing();
}

class KeyGridSpacing {
  final double btwKey = 10;
  final double btwBlock = 15;
  final double btwSections = 54;

  const KeyGridSpacing();
}
