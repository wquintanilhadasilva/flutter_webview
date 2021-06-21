import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter_inappwebview/flutter_inappwebview.dart';
import 'package:web_view/home.dart';
import 'package:web_view/store.dart';

class Login2Screen extends StatefulWidget {
  @override
  _Login2ScreenState createState() => _Login2ScreenState();
}

class _Login2ScreenState extends State<Login2Screen> {

  PullToRefreshController pullToRefreshController;
  InAppWebViewController webViewController;
  InAppWebViewGroupOptions options = InAppWebViewGroupOptions(
      crossPlatform: InAppWebViewOptions(
        useShouldOverrideUrlLoading: true,
        mediaPlaybackRequiresUserGesture: false,
      ),
      android: AndroidInAppWebViewOptions(
        useHybridComposition: true,
      ),
      ios: IOSInAppWebViewOptions(
        allowsInlineMediaPlayback: true,
      ));

  @override
  void initState() {
    super.initState();

    pullToRefreshController = PullToRefreshController(
      options: PullToRefreshOptions(
        color: Colors.blue,
      ),
      onRefresh: () async {
        if (Platform.isAndroid) {
          webViewController?.reload();
        } else if (Platform.isIOS) {
          webViewController?.loadUrl(
              urlRequest: URLRequest(url: await webViewController?.getUrl()));
        }
      },
    );
  }



  @override
  Widget build(BuildContext context) {
    return WillPopScope(
      // onWillPop: () => Future.value(false), // tira o gesto de voltar no dispositivo
        child: Scaffold(
          appBar: AppBar(
            automaticallyImplyLeading: false, // tira o botão voltar
          ),
          body: InAppWebView(
            initialUrlRequest: URLRequest(url: Uri.parse("https://192.168.15.8:5001/auth/login")), // https://192.168.15.13:5001 https://inappwebview.dev/")),
            initialOptions: options,
            pullToRefreshController: pullToRefreshController,
            onReceivedServerTrustAuthRequest: (controller, challenge) async {
              print(challenge);
              return ServerTrustAuthResponse(action: ServerTrustAuthResponseAction.PROCEED);
            },
            onWebViewCreated: (controller) {
              webViewController = controller;
            },
            onUpdateVisitedHistory: (controller, url, androidIsReload) async{
              print(url);
              print(androidIsReload);
              print(webViewController);
              await _checkUrl(url);
            }
          ),
        )
    );
  }

  void _checkUrl(Uri url) async {
    if(url.toString() == "https://192.168.15.8:5001/login-success"){

      CookieManager cookieManager = CookieManager.instance();
      List<Cookie> cookies = await cookieManager.getCookies(url: url);
      var s = Store();
      s.cookies = cookies;
      print(cookies);
      webViewController.loadUrl(urlRequest: URLRequest(url: Uri.parse("https://192.168.15.8:5001/auth/signoutlocal")));
      Navigator.pushReplacement(
          context,
          MaterialPageRoute(builder: (context) => HomeView())
      );
    }
  }
}
