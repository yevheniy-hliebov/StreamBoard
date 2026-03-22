import 'package:flutter/material.dart';

class For {
  static List<Widget> generateChildren(
    int count, {
    required List<Widget> Function(int index) generator,
  }) {
    final List<Widget> widgetList = <Widget>[];
    for (int i = 0; i < count; i++) {
      widgetList.addAll(generator(i));
    }
    return widgetList;
  }

  static List<Widget> fromList<T>({
    required List<T> items,
    required List<Widget> Function(T item) generator,
  }) {
    final List<Widget> widgetList = <Widget>[];
    for (var item in items) {
      widgetList.addAll(generator(item));
    }
    return widgetList;
  }
}
