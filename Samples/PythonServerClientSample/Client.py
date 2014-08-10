# -*- coding: utf-8 -*-

from http import client

if __name__ == '__main__':
    conn = client.HTTPConnection("127.0.0.1:8080")   #请求http服务器，这里的ip.ip.ip.ip要换成服务器端所在ip
    print("requesting...")
    
    conn.request("GET", "/")    #发出GET请求并制定请求的文件路径
    r1 = conn.getresponse()
    print(r1.status, r1.reason)             #打印响应码和响应状态信息
    
    try:
        data1 = r1.read()                   #读响应内容
    except:
        print("exception!")
    finally:
        print("read response!")
    print(data1.decode())                            #打印响应内容
    
    conn.close()                            #关闭连接
