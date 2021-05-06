

import 'package:flutter_inappwebview/flutter_inappwebview.dart';

class Store {

  static final Store _singleton = Store._internal();

  List<Cookie> _cookies = [];

  factory Store() {
    return _singleton;
  }

  Store._internal();

  void set cookies(List<Cookie> c) {
    this._cookies = c;
  }

  List<Cookie> get cookies {
    return this._cookies;
  }

}