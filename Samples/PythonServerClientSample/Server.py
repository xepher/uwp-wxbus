# -*- coding: utf-8 -*-

from http.server import HTTPServer, BaseHTTPRequestHandler
import io, shutil

class MyHttpHandler(BaseHTTPRequestHandler):
    def do_GET(self):                     #响应GET请求
        print(self.path)                  #打印客户端请求GET的路径
        enc="utf-8" 
        self.send_response(200)           #发送200状态码，表示处理正常
        self.send_header("Content-type", "text/html; charset=%s" % enc)   #发送html头，这里可说明文件类型和字符集等信息
        f = open("blacklist.txt","r")     #只读打开一个文件
        strs = f.read()                   #读出文件
        self.send_header("Content-Length", str(len(strs)))    #发送html头   说明文件长度 注意，这里如果长度和实际长度不一致的话，后面客户端处理时就会触发IncompleteRead 这个异常。
        self.end_headers()                #html头部分结束
        self.wfile.write(strs.encode())   #以刚才读出的那个文件的内容作为后续内容发出给http客户端

if __name__ == '__main__':
    server_address = ('',8080)
    handler_class = MyHttpHandler
    
    httpd=HTTPServer(server_address, handler_class)
    print("Server started port 8080...")
    httpd.serve_forever()  #启动http服务器

