import 'dart:io';

import 'package:flutter/material.dart';
import 'package:web_view/home.dart';
import 'package:webview_flutter/webview_flutter.dart';


class LoginView extends StatefulWidget {
  @override
  _LoginViewState createState() => _LoginViewState();
}

class _LoginViewState extends State<LoginView> {

  WebViewController _controller;

  @override
  void initState() {
    super.initState();
    // Enable hybrid composition.
    // if (Platform.isAndroid) WebView.platform = SurfaceAndroidWebView();
  }

  @override
  Widget build(BuildContext context) {
    String loginUrl = "someservise.com/auth";

    return WebView(
      initialUrl: 'https://www.google.com/',
      javascriptMode: JavascriptMode.unrestricted,
      onWebViewCreated: (WebViewController webViewController) {
        _controller = webViewController;
      },
      onPageFinished: (url) {
        _controller.evaluateJavascript("console.log('Hello')");
      },
      navigationDelegate: (action) async {
        print("NAVIGATION");
        print(action.url);
        if (action.url ==
            "https://stackoverflow.com/users/login?ssrc=head&returnurl=https%3a%2f%2fstackoverflow.com%2f") {
          return NavigationDecision.prevent;
        } else if (action.url == "https://www.basis.com.br/"){

          final String cookies = await _controller.evaluateJavascript('document.cookie');
          print(cookies);
          // List<Cookie> cookies = await cookieManager.getCookies(action.url);
          // cookies.forEach((cookie) {
          //   print(cookie.name + " " + cookie.value);
          // });

          return Navigator.pushReplacement(
              context,
              MaterialPageRoute(builder: (context) => HomeView())
          );
        }
        else{
          return NavigationDecision.navigate;
        }
      },
      debuggingEnabled: true,
      javascriptChannels: Set.from([
        JavascriptChannel(
            name: 'Print',
            onMessageReceived: (JavascriptMessage message) {
              print(message.message);
            })
      ]),
    );
  }
}
