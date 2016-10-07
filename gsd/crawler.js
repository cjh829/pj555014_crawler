var page = require('webpage').create();
var fs = require('fs');

page.onError = function (msg, trace) {
    console.log(msg);
    trace.forEach(function(item) {
        console.log('  ', item.file, ':', item.line);
    });
};

page.loadFinishedflag = false;

page.onLoadFinished = function(status) {
	var currentUrl = page.evaluate(function() {
		return window.location.href;
	});
	if (currentUrl == 'http://www.pj555014.com/')
	{
		page.loadFinishedflag = true;
	}
	console.log(currentUrl + ' load finished');
};

page.open('http://www.pj555014.com/', function() {

	page.includeJs('//ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js',function() {
		var imgobj = page.evaluate(function() {
			function getImgDimensions($i) {
				return {
					top : $i.offset().top,
					left : $i.offset().left,
					width : $i.width(),
					height : $i.height()
				}
			}
			var warpper = window.parent.frames[0].document;
			$('#user-login .btn-pos-refresh',warpper).click();
			return getImgDimensions($('#checkNum_img', warpper));
		});
				
		page.clipRect = imgobj;
		
		function checkPage3() { //login success
			if (page.loadFinishedflag != true)
			{
				console.log('checkPage3...fail');
				setTimeout(checkPage3,2000);
				return;
			}

			var login_window = page.evaluate(function () {
			    var warpper = window.parent.frames[0].document;
			    return $('#user-logined', warpper).length;
			});

			if (login_window < 1) {
			    console.log('checkPage3...fail due to incorrect page!');
			    page.loadFinishedflag = false;
			    setTimeout(checkPage3, 2000);
			    return;
			}

			console.log('checkPage3...pass');
			console.log('render 3');
			page.render('3.png');
			
			var moneys = page.evaluate(function() {
				var warpper = window.parent.frames[0].document;
				var moneyobj = {
					'lebo' : $('#lebo_money',warpper).text(),
					'bb' : $('#bb_money',warpper).text(),
					'mg' : $('#mg_money',warpper).text(),
					'ag' : $('#ag_money',warpper).text(),
					'ct' : $('#ct_money',warpper).text(),
					'ot' : $('#ot_money',warpper).text()
				};
				
				return moneyobj
			});
			moneys.time = Date.now();
			var moneyText = JSON.stringify(moneys);
			console.log(moneyText);
			
			fs.write('money.txt',moneyText,'w');
			
			phantom.exit();
		}
		
		function checkPage2() {  //aggrement page
			if (page.loadFinishedflag != true)
			{
				console.log('checkPage2...fail');
				setTimeout(checkPage2,2000);
				return;
			}

			var formCount = page.evaluate(function () {
			    var warpper = window.parent.frames[0].document;
			    return $('#content form', warpper).length;
			});

			if (formCount < 2)
			{
			    console.log('checkPage2...fail due to incorrect page!');
			    page.loadFinishedflag = false;
			    setTimeout(checkPage2, 2000);
			    return;
			}

			console.log('checkPage2...pass');
			console.log('render 2');
			page.render('2.png');
			page.loadFinishedflag = false; //reset Flag;
			page.evaluate(function() {
			    var warpper = window.parent.frames[0].document;
			    $('#content form', warpper).each(function () {
			        var postTarget = $(this).prop('action');
			        if (postTarget.indexOf("logout.php") < 0) { //to find right "enter button"
			            $('.btn_001', $(this)).click();
			            return false; //break each;
			        }
			    });
			});
			checkPage3();
		}
		
		function checkOCRResult() { //login page
			console.log('checkOCR...');
			if (!fs.exists('ocr.txt'))
			{
				console.log('checkOCR...fail');
				setTimeout(checkOCRResult,2000);
				return;
			}
			console.log('checkOCR...pass');
			var content = fs.read('ocr.txt');
			page.clipRect = { left:0, top:0, width:0, height:0 };
			console.log('render 1');
			page.render('1.png');
			page.loadFinishedflag = false; //reset Flag;
			page.evaluate(function (vcode, username, password) {
			    var warpper = window.parent.frames[0].document;
			    $('#username', warpper).val(username); //defined by CrawlerJob.cs
			    $('#passwd', warpper).val(password);  //defined by CrawlerJob.cs
			    $('#vlcodes', warpper).val(vcode);
			    $('#user-login .input-login', warpper).click();
			}, content, login_username, login_password);
			
			checkPage2();
		}
		
		setTimeout(function() { //waiting captcha png to load
			page.render('captcha.png');
			checkOCRResult();
		},2000);
	});
});