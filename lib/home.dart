import 'package:flutter/material.dart';

class HomeView extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Login com sucesso'),
        automaticallyImplyLeading: false,
      ),
      body: Center(
        child: Text('Logado com sucesso'),
      ),
    );
  }
}
