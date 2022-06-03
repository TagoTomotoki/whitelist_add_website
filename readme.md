whitelist_add_website是一款可以让你通过简单的GET请求完成云主机白名单添加的一个工具。通过本工具，你可以使用类似以下链接来为指定主机添加白名单

```
http://主机名:port/getip?key=111111
```

其中，key为必须的密钥参数，用来为加白行为鉴权

port为可配置的端口

如果添加成功，将会有以下提示

> ```
> rule update,ip:
> ```

## 启动教程

本程序依赖于`iptables`命令进行白名单添加，因此，请在使用前配置好系统`iptables`并确保可用

### 参数配置

首先，你需要创建一个配置文件来存储相应参数

输入以下命令

```
vi ./whitelist-key
```

编辑器会自动创建文件并打开，此时按下i键，进入编辑模式。添加以下参数

```
Key=
Port=
ListenPort=
```

`Key=`鉴权密钥，体现在url中
`Port=`需要加白才能访问的端口
`ListenPort=`本程序监听相应请求的端口

请注意，Key中不能带中文及特殊字符，否则程序会报错，大小写敏感。

编辑好之后，按下esc键，然后输入

```
:wq
```

保存

在以上配置文件生成的路径，运行以下命令

### x64

```bash
wget https://github.com/TagoTomotoki/whitelist_add_website/releases/download/v2.0/whitelist
chmod +x ./whitelist
./whitelist
```

### arm

```bash
wget https://github.com/TagoTomotoki/whitelist_add_website/releases/download/v2.0/whitelist-arm
chmod +x ./whitelist-arm
./whitelist-arm
```

第一次运行，会自动配置systemd系统服务，添加完毕后，程序会自动启动配置完成的服务，之后就可以访问监听端口进行白名单添加操作

你可以使用`systemctl status whitelist`来查看服务状态

## cloudflare支持

程序支持读取header中的`CF-Connecting-IP`字段，因此可以接受来自cloudflare代理后传递的真实ip，这有利于在公网环境下隐藏url中的鉴权密钥

### 如何使用这一特性

cloudflare支持80端口回源，因此只要把ssl设置为“灵活”，将ListenPort设置为80即可

## 使用解析域名的方式更新白名单

本工具支持以域名方式更新白名单，请注意，域名解析的ip不等于来源机器的出站ip

### 实现

请求添加mode=dns字段，并添加相应域名信息，如

```
http://主机名:port/getip?key=111111&mode=dns&host=www.baidu.com
```

## 卸载及配置重置

请运行以下命令

```
iptables -t filter -F WHITELIST
iptables -D INPUT -j WHITELIST
iptables -t filter -X WHITELIST
rm -rf /etc/systemd/system/whitelist.service
systemctl stop whitelist
systemctl daemon-reload
```

而配置重置的话，执行上面最后三条命令，并在配置文件的目录内

运行`./whitelist`即可