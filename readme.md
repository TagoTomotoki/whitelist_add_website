本工具由iptables实现，使用前请先禁用firewalld等其他防火墙，并添加以下规则：  
iptables -A INPUT -i lo -j ACCEPT  
iptables -A INPUT -m state --state ESTABLISHED,RELATED -j ACCEPT  
iptables -A INPUT -p tcp --dport 22 -j ACCEPT  
iptables -P INPUT DROP  
下载Releases到当前目录，并赋予运行权限  
wget https://github.com/TagoTomotoki/whitelist_add_website/releases/download/v1.0/whitelist  
chmod +x ./whitelist  
使用screen后台运行  
screen -S  
./whitelist --urls=http://0.0.0.0:5000 --ASPNETCORE_ENVIRONMENT=Production  
Ctrl+ad,切换回原终端。  
添加白名单，访问 http://ip:5000/getip 即可。  
附带32位强密码生成器 http://ip:5000/randnumber  
