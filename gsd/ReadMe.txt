==== 说明文档 ====

需求：

http://www.pj555014.com 
在上面网站注册个新账号，写个自动登录客户端，定时获取注册账号的额度。 

使用的外部lib：
 - PhantomJS - 模拟Browser爬网站，点选输入等操作
 - Tesseract - 把验证码图片解析为文字
 - Quartz    - 定时执行(每分钟执行一次)


程式流程：
(1) PhantomJS 爬网站，点选捞回验证码，存為captcha.png
(2) FileWatcher 侦测到 captcha.png，利用自己写的process函式去图片杂点，再交给Tesseract解析文字，写入ocr.txt
(3) PhantomJS 侦测到 ocr.txt，读取内容得到正确验证码，模拟输入帐号、密码、验证码並点选sumbit按钮
(4) 进条款宣告页，模拟点选「同意」
(5) 回首页，且已登入状态，捞取额度并用JSON格式存为money.txt，供其他程式/后续使用