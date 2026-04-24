import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:streamboard/core/theme/app_theme.dart';
import 'package:streamboard/features/home/presentaions/screens/home_screen.dart';
import 'package:streamboard/features/home/providers/deck_provider.dart';
import 'package:streamboard/features/home/services/grid_service.dart';
import 'package:streamboard/features/home/services/websocket_service.dart';
import 'package:streamboard/features/settings/providers/settings_provider.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  SystemChrome.setEnabledSystemUIMode(SystemUiMode.immersiveSticky);

  final prefs = await SharedPreferences.getInstance();

  runApp(
    MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => SettingsProvider(prefs)),

        // Надаємо GridService
        ProxyProvider<SettingsProvider, GridService>(
          update: (context, settings, previous) {
            return GridService(baseUrl: settings.baseUrl);
          },
        ),

        ProxyProvider<SettingsProvider, WebSocketService>(
          create: (_) => WebSocketService(),
          update: (context, settings, previous) {
            final ws = previous ?? WebSocketService();

            if (settings.isConfigured) {
              ws.connect(settings.baseUrl);
            } else {
              ws.disconnect();
            }
            return ws;
          },
          dispose: (context, ws) => ws.dispose(),
        ),

        ChangeNotifierProxyProvider2<
          GridService,
          WebSocketService,
          DeckProvider
        >(
          create: (context) => DeckProvider(
            context.read<GridService>(),
            context.read<WebSocketService>(),
          ),
          update: (context, gridService, wsService, previous) {
            if (previous == null) return DeckProvider(gridService, wsService);
            previous.updateServices(gridService, wsService);
            return previous;
          },
        ),
      ],
      child: const App(),
    ),
  );
}

class App extends StatelessWidget {
  const App({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'StreamBoard Application',
      darkTheme: AppTheme.dark,
      themeMode: ThemeMode.dark,
      debugShowCheckedModeBanner: false,
      theme: ThemeData(colorScheme: .fromSeed(seedColor: Colors.deepPurple)),
      home: const HomeScreen(),
    );
  }
}
