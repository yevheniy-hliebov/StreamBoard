class GridLayout {
  final String name;
  final int columns;
  final int rows;

  GridLayout({required this.name, required this.columns, required this.rows});

  static GridLayout fromJson(Map<String, dynamic> json) {
    return GridLayout(
      name: json['name'],
      columns: json['columns'],
      rows: json['rows'],
    );
  }
}
