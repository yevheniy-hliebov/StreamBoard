import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:streamboard/common/widgets/orientation_adaptive_scaffold.dart';
import 'package:streamboard/core/constants/spacing.dart';
import 'package:streamboard/features/settings/providers/settings_provider.dart';

class SettingsScreen extends StatefulWidget {
  const SettingsScreen({super.key});

  @override
  State<SettingsScreen> createState() => _SettingsScreenState();
}

class _SettingsScreenState extends State<SettingsScreen> {
  final _formKey = GlobalKey<FormState>();
  late TextEditingController _addressController;
  late TextEditingController _portController;

  @override
  void initState() {
    super.initState();
    final settings = context.read<SettingsProvider>();
    _addressController = TextEditingController(text: settings.address);
    _portController = TextEditingController(text: settings.port);
  }

  @override
  void dispose() {
    _addressController.dispose();
    _portController.dispose();
    super.dispose();
  }

  void _save() {
    if (_formKey.currentState!.validate()) {
      context.read<SettingsProvider>().saveSettings(
        _addressController.text.trim(),
        _portController.text.trim(),
      );
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(const SnackBar(content: Text('Settings saved')));
      Navigator.pop(context);
    }
  }

  @override
  Widget build(BuildContext context) {
    return OrientationAdaptiveScaffold(
      title: 'Settings',
      leading: IconButton(
        icon: const Icon(Icons.arrow_back),
        onPressed: () => Navigator.pop(context),
      ),
      body: Padding(
        padding: const EdgeInsets.all(Spacing.md),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                'Server Connection',
                style: Theme.of(context).textTheme.titleMedium,
              ),
              const SizedBox(height: Spacing.md),
              TextFormField(
                controller: _addressController,
                decoration: const InputDecoration(
                  labelText: 'IP Address / Hostname',
                  border: OutlineInputBorder(),
                  hintText: 'e.g., 192.168.1.2 or localhost',
                ),
                validator: (value) =>
                    value == null || value.isEmpty ? 'Cannot be empty' : null,
              ),
              const SizedBox(height: Spacing.md),
              TextFormField(
                controller: _portController,
                decoration: const InputDecoration(
                  labelText: 'Port',
                  border: OutlineInputBorder(),
                  hintText: 'e.g., 13550',
                ),
                keyboardType: TextInputType.number,
                validator: (value) =>
                    value == null || value.isEmpty ? 'Cannot be empty' : null,
              ),
              const SizedBox(height: Spacing.lg),
              SizedBox(
                width: double.infinity,
                child: FilledButton(
                  onPressed: _save,
                  child: const Text('Save & Connect'),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
