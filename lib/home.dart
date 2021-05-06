import 'package:flutter/material.dart';
import 'package:flutter_inappwebview/flutter_inappwebview.dart';
import 'package:web_view/store.dart';

class HomeView extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    var store = Store();
    return Scaffold(
      appBar: AppBar(
        title: Text('Login com sucesso'),
        automaticallyImplyLeading: false,
      ),
      body: SingleChildScrollView(child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        crossAxisAlignment: CrossAxisAlignment.start,
        children: _buildItens(store.cookies),
      ),
    ));
  }

  List<Widget> _buildItens(List<Cookie> cookies) {
    var r = cookies.map((e) => _buildItem(e)).toList();
    return r;
  }

  Widget _buildItem(Cookie c){

    return Container(
       padding: EdgeInsets.only(bottom: 10),
        child: Expanded(child:Row(
          children: [
            Flexible( child: Text(c.name)),
            Text(': '),
            Flexible( child: Text(c.value))
          ],
        )
    ));

    // return Expanded(
    //     child: Row(
    //       children: [
    //         Flexible( child: Text(c.name)),
    //         Text(': '),
    //         Flexible( child: Text(c.value))
    //       ],
    //     )
    // );
  }
}
