import 'dart:convert';
import 'dart:typed_data';
import 'package:http/http.dart' as http;

class GridService {
  final String baseUrl;

  GridService({required this.baseUrl});

  Future<Map<String, dynamic>> getButtons() async {
    try {
      final response = await http.get(Uri.parse('$baseUrl/api/grid/buttons'));

      if (response.statusCode == 200) {
        return jsonDecode(response.body);
      }
      throw Exception(
        'Failed to load buttons. Status: ${response.statusCode}. Body: ${response.body}',
      );
    } catch (e) {
      throw Exception('Network error: $e');
    }
  }

  Future<Uint8List?> getImage(String keyCode) async {
    final response = await http.get(
      Uri.parse('$baseUrl/api/grid/$keyCode/image'),
    );

    if (response.statusCode == 200) {
      return response.bodyBytes;
    }
    return null;
  }

  Future<void> clickButton(String keyCode) async {
    final response = await http.post(Uri.parse('$baseUrl/api/grid/$keyCode'));

    if (response.statusCode != 200) {
      throw Exception('Failed to click button');
    }
  }
}
