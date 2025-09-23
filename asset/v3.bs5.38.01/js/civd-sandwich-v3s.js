/*
CIVD template V3 "sandwich" page
version : v3.0.0 (20211228)

before this javascript file, it should be included those files below :

jquery.js
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





	//==========左側選單啟用Metismenu與記錄開啟階層==========

	//==========左側選單啟用Metismenu與記錄開啟階層 End==========





	//==========跨頁面記錄offcanvas啟動狀態 ==========
	// isMainNavActive
	// 先判斷是否已有值
	// console.log( "進入此頁前: isMainNavActive = " +  sessionStorage.getItem('isMainNavActive') )
	if( sessionStorage.getItem('isMainNavActive') === null){
		// console.log("進入此頁判斷是否有 isMainNavActive 為 null");
	   
	   if( $("body").hasClass("offcanvas-active") ){
			sessionStorage.setItem('isMainNavActive', true);
		    // console.log("載入頁面後，偵測到null 然後放true");  
	   }else{
			sessionStorage.setItem('isMainNavActive', false);
		    // console.log("載入頁面後偵測到null 然後放false");
	   }
	   
	 }else{
	//    console.log("進入此頁判斷是否有 isMainNavActive 有東西(true or false)");  
		 
	   // 取出記錄中的 isMainNavActive
	   var _isMainNavActive = sessionStorage.getItem('isMainNavActive');
	//    console.log( "判斷為:" + _isMainNavActive );
	   
	   if(_isMainNavActive === 'true'){

		   //載入記錄之前，將offcanvas動畫取消 cancel transition
		   $('#page-container').addClass("transition-none");
		   $('#main-nav').addClass("transition-none");


		   $("body").addClass("offcanvas-active");
		//    console.log("載入頁面後，偵測到有值為true 然後body 加上 offcanvas-active");
		   

		  //確定之後，回覆動畫
          setTimeout(function () {
			$('#page-container').removeClass("transition-none");
			$('#main-nav').removeClass("transition-none");			
		  }, 500);
		  
	   }else if( _isMainNavActive === 'false' ){
		   $("body").removeClass("offcanvas-active");
		//    console.log("載入頁面後，偵測到有值為false 然後body 刪除 offcanvas-active");
	   }
	 }
	 //==========跨頁面記錄offcanvas啟動狀態 End ==========





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



	