# 我敢打赌你会在看完我的猫咪后去洗手间
因FB留言區充斥著不少 `我敢打赌你会在看完我的视频后去洗手间` 等相關廣告連結，故寫了這個後端來對這類留言來回應

已架設的網站網址，可自行複製後貼到其他地方使用: https://bet.konnokai.me

截至2023/04/03的點閱數量為: 1711

# 自行架設所需環境
ASP .NET Core 6.0 Runtime

Redis

Apache或Nginx等可反向代理的WebServer，或是直接執行後到路由器開Port裸奔也行，但這樣不會有https