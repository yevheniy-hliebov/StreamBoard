import 'package:flutter/material.dart';
import 'package:mobile_scanner/mobile_scanner.dart';
import 'package:provider/provider.dart';
import 'package:streamtabula/features/settings/providers/settings_provider.dart';

class QrScannerScreen extends StatefulWidget {
  const QrScannerScreen({super.key});

  @override
  State<QrScannerScreen> createState() => _QrScannerScreenState();
}

class _QrScannerScreenState extends State<QrScannerScreen> {
  bool _isScanned = false;
  // ДОДАНО: Створюємо контролер у State
  late final MobileScannerController _controller;
  // Централізований розмір зони сканування (квадрат по центру)
  static const double _scanAreaSize = 250.0;

  @override
  void initState() {
    super.initState();
    // Ініціалізуємо контролер (можна додати налаштування, наприклад, force_torch)
    _controller = MobileScannerController();
  }

  @override
  void dispose() {
    // Не забудьте знищити контролер
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Scan QR Code')),
      // ДОДАНО: Wrap Stack with LayoutBuilder
      // Це потрібно, щоб розрахувати Rect вікна сканування ДО побудови сканера
      body: LayoutBuilder(
        builder: (context, constraints) {
          // 1. Розраховуємо Rect вікна сканування (frame) у екранних координатах
          // Він має точнісінько збігатися з вирізом у CustomPaint
          final left = (constraints.maxWidth - _scanAreaSize) / 2;
          final top = (constraints.maxHeight - _scanAreaSize) / 2;
          final frameRect = Rect.fromLTWH(
            left,
            top,
            _scanAreaSize,
            _scanAreaSize,
          );

          return Stack(
            children: [
              // 2. Шар камери з обмеженням вікна
              MobileScanner(
                controller: _controller,
                // ДОДАНО: Обмежуємо зону сканування розрахованим Rect
                // mobile_scanner автоматично переведе його в координати камери
                scanWindow: frameRect,
                onDetect: (capture) {
                  if (_isScanned) return;

                  final List<Barcode> barcodes = capture.barcodes;
                  for (final barcode in barcodes) {
                    final String? rawValue = barcode.rawValue;

                    if (rawValue != null &&
                        (rawValue.startsWith('http://') ||
                            rawValue.startsWith('https://'))) {
                      _isScanned = true;

                      final settings = context.read<SettingsProvider>();
                      settings.updateServerAddress(rawValue);

                      if (mounted) {
                        Navigator.of(context).pop();

                        ScaffoldMessenger.of(context).showSnackBar(
                          const SnackBar(
                            content: Text('Server address updated!'),
                          ),
                        );
                      }
                      break;
                    }
                  }
                },
              ),

              // 3. Шар затемнення з "віконцем" (передаємо розмір зони)
              _ScannerOverlay(scanAreaSize: _scanAreaSize),
            ],
          );
        },
      ),
    );
  }
}

// Віджет для керування шаром затемнення (майже без змін, але приймає size)
class _ScannerOverlay extends StatelessWidget {
  final double scanAreaSize;

  const _ScannerOverlay({required this.scanAreaSize});

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        return Stack(
          children: [
            // Кастомна мальовка фону з діркою
            CustomPaint(
              size: Size(constraints.maxWidth, constraints.maxHeight),
              painter: _ScannerOverlayPainter(scanAreaSize: scanAreaSize),
            ),

            // Рамка навколо прозорої зони
            Center(
              child: Container(
                width: scanAreaSize,
                height: scanAreaSize,
                decoration: BoxDecoration(
                  border: Border.all(
                    color: Theme.of(context).colorScheme.primary,
                    width: 2.5,
                  ),
                  borderRadius: BorderRadius.circular(16),
                ),
              ),
            ),

            // Текстова підказка
            Positioned(
              bottom: (constraints.maxHeight - scanAreaSize) / 2 - 60,
              left: 0,
              right: 0,
              child: const Text(
                'Position the QR code within the frame',
                textAlign: TextAlign.center,
                style: TextStyle(
                  color: Colors.white,
                  fontSize: 16,
                  fontWeight: FontWeight.w500,
                ),
              ),
            ),
          ],
        );
      },
    );
  }
}

// Логіка вирізання прозорого квадрата на чорному фоні (без змін)
class _ScannerOverlayPainter extends CustomPainter {
  final double scanAreaSize;

  _ScannerOverlayPainter({required this.scanAreaSize});

  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()..color = Colors.black.withOpacity(0.6);

    final screenRect = Rect.fromLTWH(0, 0, size.width, size.height);
    final backgroundPath = Path()..addRect(screenRect);

    final left = (size.width - scanAreaSize) / 2;
    final top = (size.height - scanAreaSize) / 2;
    final cutoutRect = Rect.fromLTWH(left, top, scanAreaSize, scanAreaSize);
    final cutoutRRect = RRect.fromRectAndRadius(
      cutoutRect,
      const Radius.circular(16),
    );
    final cutoutPath = Path()..addRRect(cutoutRRect);

    final overlayPath = Path.combine(
      PathOperation.difference,
      backgroundPath,
      cutoutPath,
    );

    canvas.drawPath(overlayPath, paint);
  }

  @override
  bool shouldRepaint(covariant CustomPainter oldDelegate) => false;
}
