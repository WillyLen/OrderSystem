/*
CIVD template V3 "systemMain" page
version : v3.0.0 (20211228)

before this javascript file, it should be included those files below :

jquery.js
metisMenu.js
jquery.slimscroll.min.js
*/
$(function(){


	//LocalStorage detect
	function supportLocalStorage() {
		var testmod = "hi";
		try {
			localStorage.setItem(testmod, testmod);
			localStorage.removeItem(testmod);
			return true;
		} catch (exception) {
			// console.log("your browser is not support local storage");
			return false;
		}
	}
	function ClearLocalStorage() {
		localStorage.clear();
	}


	
	//程式開始
	noteBodySmall();		//偵測手機瑩幕並註記



	//視窗大小改變時。 When screen resize
	//在使用者端會出現的情況是，改變平板垂直或水平狀態
	$( window ).resize(function() {
		noteBodySmall();

		if( $('body').hasClass("body-small")   &&  supportLocalStorage()){	
			//手機瑩幕時(768px內)，收合選單
			$('body').removeClass("offcanvas-active");
			sessionStorage.setItem('isMainNavActive', false);		
		}	
    });
    


	
	
	

	//==========偵測手機瑩幕並註記==========	
	function noteBodySmall(){
		if ($(this).width() < 769) {
			$('body').addClass('body-small');
		} else {
			$('body').removeClass('body-small');
		}
	}
	//==========偵測手機瑩幕並註記 End==========	





	//==========左側選單開啟與收合(Offcanvas)==========
	$("[data-toggle='offcanvas']").on('click', function() {
		$("body").toggleClass('offcanvas-active');

		if( $("body").hasClass("hdrsubarea-active")  ){
			$("body").removeClass("hdrsubarea-active")
		}
		offcanvasActiveCheck();
	});
	$("#btn-header-subarea").on('click', function() {
		$("body").toggleClass('hdrsubarea-active');

		if( $("body").hasClass("offcanvas-active") ){
			$("body").removeClass("offcanvas-active")
		}
		offcanvasActiveCheck();
	});
	$("#screencover-dark").click(function(){
		if( $("body").hasClass("hdrsubarea-active")  ){
			$("body").removeClass("hdrsubarea-active")
		}	
		if( $("body").hasClass("offcanvas-active") ){
			$("body").removeClass("offcanvas-active")
		}
		offcanvasActiveCheck();	
	});

	function offcanvasActiveCheck(){
		if( $("body").hasClass("offcanvas-active") ){
			sessionStorage.setItem('isMainNavActive', true);	  
		}else{
			sessionStorage.setItem('isMainNavActive', false);
		}	
	}
	//==========左側選單開啟與收合(Offcanvas) End==========









	//==========回頂端 Back to top==========
	/*
	功能
	頁面很長，捲動超過瑩幕範圍時，出現回頁面頂端的連結
	When you scroll a long page, it will show a link to back to top.
	*/
	
	var windowHeight = $(window).height();
	var $back_to_top = $("#back-to-top");
	var $window = $(window);

	// 當網頁捲軸捲動時
	$window.scroll(function(){
		
		//如果不是手機瑩幕則會出現back-to-top
		if( !$("body").hasClass("body-small") ){
			//往下捲動 顯示回頁首
			if( $window.scrollTop() > 100 ){
				$back_to_top.css("display","block");
			}
			
			//往上捲動，捲至快頂時 隱藏回頁首
			if( $window.scrollTop() <= 100 ){
				$back_to_top.css("display","none");		
			}
		}// body-small End
	});	
	//==========回頂端 Back to top   END==========


});




