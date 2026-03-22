import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import 'package:streamboard/core/theme/app_theme.dart';
import 'package:streamboard/features/home/presentaions/screens/home_screen.dart';
import 'package:streamboard/features/home/providers/deck_provider.dart';
import 'package:streamboard/features/home/services/grid_service.dart';

void main() {
  WidgetsFlutterBinding.ensureInitialized();
  SystemChrome.setEnabledSystemUIMode(SystemUiMode.immersiveSticky);

  runApp(
    MultiProvider(
      providers: [
        Provider<GridService>(
          create: (_) => GridService(baseUrl: 'http://192.168.1.2:13550'),
        ),
        ChangeNotifierProxyProvider<GridService, DeckProvider>(
          create: (context) => DeckProvider(context.read<GridService>()),
          update: (context, service, previous) =>
              previous ?? DeckProvider(service),
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
