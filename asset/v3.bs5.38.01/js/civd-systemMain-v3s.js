/*
CIVD template V3 "systemMain" page
version : v3.0.0.a1 (20211228) - hide console log

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
    



	/*Page Loading 不使用*
	//==========Page Loading==========
	//頁面載入前，先出現Loading畫面，載完後隱藏。目前有2個Loading隨機出現。
	function getRandomInt(max) {
  		return Math.floor(Math.random() * Math.floor(max)) + 1;
		//隨機產生 output: 1, 2,... max - 1
	}

	var loading_style_total = 2;	//Loading樣式總數量
	var loading_style = "loading-style0" + getRandomInt(loading_style_total);
	console.log("Selected loading style = " + loading_style);
	
	$(".loading-style").each(function() {
		if( $(this).attr("id") != loading_style){
			$(this).addClass("ld-hide");		
		}
	});
	
	window.addEventListener("load", function(){
		const loader = document.querySelector("#civd-loading-cover");
		if( loader != null && loader != undefined){
			loader.className += "ld-hide";
		}
	});
	//==========Page Loading End==========	
	
	
	//如需某個動作執行後，出現Loading畫面，則大致寫法如下
	//dosomething{
	//	 if on removeClass, if off addClass
	//   $("#civd-loading-cover").toggleClass("ld-hide");
	//}
	/**/
	
	
	

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
	//如果AP採用版型預設的選單，則啟動metismenu.js 與 VD自製記錄選單開啟階層程式;如不採用則略過。
	if( !$('#main-nav-menu.metismenu').length == 0 ){
		$('#main-nav-menu.metismenu').metisMenu().civdMetisMenuRecord();
	}
	//==========左側選單啟用Metismenu與記錄開啟階層 End==========





	//==========跨頁面記錄offcanvas啟動狀態 ==========
	// isMainNavActive
	// (for PC version)先判斷是否已有值
	// console.log( "進入此頁前: isMainNavActive = " +  sessionStorage.getItem('isMainNavActive') )
	if( sessionStorage.getItem('isMainNavActive') === null){
		// console.log("進入此頁判斷是否有 isMainNavActive 為 null");
	   
	   if( $("body").hasClass("offcanvas-active") && !$("body").hasClass("body-small") ){
			sessionStorage.setItem('isMainNavActive', true);
		    // console.log("[0.1] -> 載入頁面後，偵測到null 然後放true");  
	   }else if( !$("body").hasClass("offcanvas-active") && !$("body").hasClass("body-small") ){
			sessionStorage.setItem('isMainNavActive', false);
		    // console.log("[0.2] -> 載入頁面後偵測到null 然後放false");
	   }
	   
	 }else{
	//    console.log("[1.0] -> 進入此頁判斷是否有 isMainNavActive 有東西(true or false)");  
		 
	   // 取出記錄中的 isMainNavActive
	   var _isMainNavActive = sessionStorage.getItem('isMainNavActive');
	//    console.log( "判斷為:" + _isMainNavActive );
	   
	   if( (_isMainNavActive === 'true') && !$("body").hasClass("body-small") ){

		   //載入記錄之前，將offcanvas動畫取消 cancel transition
		   $('#page-container').addClass("transition-none");
		   $('#main-nav').addClass("transition-none");


		   $("body").addClass("offcanvas-active");
		//    console.log("[1.1] -> 載入頁面後，偵測到有值為true 然後body 加上 offcanvas-active");
		   

		  //確定之後，回覆動畫
          setTimeout(function () {
			$('#page-container').removeClass("transition-none");
			$('#main-nav').removeClass("transition-none");			
		  }, 500);
		  
	   }else if( (_isMainNavActive === 'false') && !$("body").hasClass("body-small") ){
		   $("body").removeClass("offcanvas-active");
		//    console.log("[1.2] -> 載入頁面後，偵測到有值為false 然後body 刪除 offcanvas-active");
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





	// test text change
	$("#changeTextSize").click(function(){
		$("html").toggleClass("basefont-large");
	});
});





/*
made by CIVD (China Airlines Visual Design team)
version 3.0.0
目的: MetisMenu的擴充，記錄目前選單收合位置。
前置: metismenu.js
*/
(function( $ ){

	$.fn.civdMetisMenuRecord = function() {
		return this.each(function() {
			var $this   = $(this);
			//console.log(	supportLocalStorage()	 )
	
	
		
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
			
			if ( supportLocalStorage() ) {
				// console.log("support local storage");
				// console.log( 'the data save :' + sessionStorage.menulevel );

				//set menu level hierarchy (level tree) into <li>
				$this.find("li").each(function(index){
					var currentIndex = $( this ).closest( "ul" ).children().index(this)+1;
			
					var parentlevel = $( this ).parentsUntil( ".menu", "li" ).attr("data-menulevel");
			
					var levelID = '';
			
					if( typeof parentlevel === 'undefined' ){
						levelID = currentIndex;
					}else{
						levelID = parentlevel + '-' + currentIndex;
					}
			
					$(this).attr("data-menulevel",levelID);
					
				}).click(function(event){
							
					sessionStorage.menulevel = $( this ).attr("data-menulevel");
					event.stopPropagation();	//停止往前回遡父階層
				});//<li> End
				
				
				//判斷是否先前已存入資料
				if( typeof sessionStorage.menulevel !== 'undefined' ){
					// console.log('有存入資料');
					
					$this.find("li.mm-active").removeClass("mm-active").children("ul").removeClass("mm-show");
					var savedMenuLevel = sessionStorage.menulevel;
					var savedMenuLevel_array = savedMenuLevel.split("-");
					
					// console.log("savedMenuLevel:" + savedMenuLevel + " array:" +  savedMenuLevel_array + "  array.length = " +  savedMenuLevel_array.length);
					
					//取得先前的值，依序拆解並指定開啟
					for(var i = 0; i < savedMenuLevel_array.length - 1 ; i++){
						if( i === 0){
							//level 1
							$this.children( "li:eq(" + (savedMenuLevel_array[i] - 1) +")" ).addClass("mm-active")
								 .children("ul").addClass("mm-show");
						}else if ( i > 0 ){
							//level 2 or more
							$this.find("li.mm-active").eq( i - 1 ).children("ul")
								 .children( "li:eq(" + (savedMenuLevel_array[i] - 1) +")" ).addClass("mm-active")
								 .children("ul").addClass("mm-show");
						}//end if
					}// end for loop
									
				}// end if storage not undefined( it means it has data)
				
			}// end if supportLocalStorage()
		});
	};//civdMetisMenuRecord End	
})(jQuery);
/**/
	