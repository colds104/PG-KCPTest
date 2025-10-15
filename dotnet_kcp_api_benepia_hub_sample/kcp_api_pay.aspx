<%@ Page Language="C#" AutoEventWireup="true" CodeFile="./kcp_api_pay.aspx.cs" Inherits="Payment.Kcp_api_pay" %>
<!DOCTYPE html>
<html>
<head>
    <title>*** NHN KCP API SAMPLE ***</title>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="x-ua-compatible" content="ie=edge"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0, user-scalable=yes, target-densitydpi=medium-dpi">
    <link href="./static/css/style.css" rel="stylesheet" type="text/css"/>
    
</head>
<body oncontextmenu="return false;">
    <div class="wrap">
        <!-- header -->
        <div class="header">
            <a href="index.html" class="btn-back"><span>뒤로가기</span></a>
            <h1 class="title">TEST SAMPLE</h1>
        </div>
        
                <!-- //header -->
        <!-- contents -->
        <div id="skipCont" class="contents">
            <h2 class="title-type-3">요청  DATA</h2>
            <ul class="list-type-1">
                <li>
                    <div class="left">
                        <p class="title"></p>
                    </div>
                    <div class="right">
                        <div class="ipt-type-1 pc-wd-3">
                            <textarea style="height:200px; width:450px" readonly><%=req_param%></textarea>
                        </div>
                    </div>
                </li>
            </ul>
            <h2 class="title-type-3">응답  DATA </h2>
            <ul class="list-type-1">
                <li>
                    <div class="left">
                        <p class="title"></p>
                    </div>
                    <div class="right">
                        <div class="ipt-type-1 pc-wd-3">
                            <textarea style="height:200px; width:450px" readonly><%=res_param%></textarea>
                        </div>
                    </div>
                </li>
            </ul>
            <h2 class="title-type-3">응답  DATA </h2>
            <ul class="list-type-1">
                <li>
                    <div class="left"><p class="title">결과코드</p></div>
                    <div class="right">
                        <div class="ipt-type-1 pc-wd-2">
                            <%=res_cd %>
                        </div>
                    </div>
                </li>
                <li>
                    <div class="left"><p class="title">결과메세지</p></div>
                    <div class="right">
                        <div class="ipt-type-1 pc-wd-2">
                            <%=res_msg %><br/>
                        </div>
                    </div>
                </li>
 <%
       /* ============================================================================== */
       /* =   1. 정상 조회 시 결과 출력 ( res_cd값이 0000인 경우)                  			   = */
       /* = -------------------------------------------------------------------------- = */
       if ( "query".Equals ( req_tx ))
       {  
    	  if("0000".Equals ( res_cd ))
    	 {
				
%>        
             <li>
                 <div class="left"><p class="title">사용 가능 포인트</p></div>
                   <div class="right">
                      <div class="ipt-type-1 pc-wd-2">
                         <%=rsv_pnt %>
                        </div>
                  </div>
            </li>    
            </ul>
 
 <%
          }
       }       
 
	 /* ============================================================================== */
	 /* =   1. 정상 결제 시 결과 출력 ( res_cd값이 0000인 경우)                  		     = */
	 /* = -------------------------------------------------------------------------- = */
 
     if ( "pay".Equals ( req_tx ))
      {
    	 
    	 if("0000".Equals ( res_cd ))
   	  {
    
%>           

                <li>
                    <div class="left"><p class="title">NHN KCP 거래번호</p></div>
                    <div class="right">
                        <div class="ipt-type-1 pc-wd-2">
                            <%=tno %>
                        </div>
                    </div>
                </li>
                <li>
                    <div class="left"><p class="title">총 결제금액</p></div>
                    <div class="right">
                        <div class="ipt-type-1 pc-wd-2">
                            <%=pnt_amount %>
                        </div>
                    </div>
                </li>
                <li>
                    <div class="left"><p class="title">승인시각</p></div>
                    <div class="right">
                        <div class="ipt-type-1 pc-wd-2">
                            <%=pnt_app_time %>
                        </div>
                    </div>
                </li>
                 <li>
                    <div class="left"><p class="title">승인번호</p></div>
                    <div class="right">
                        <div class="ipt-type-1 pc-wd-2">
                            <%=pnt_app_no %>
                        </div>
                    </div>
                </li>
                 <li>
                    <div class="left"><p class="title">잔여포인트</p></div>
                    <div class="right">
                        <div class="ipt-type-1 pc-wd-2">
                            <%=rsv_pnt %>
                        </div>
                    </div>
                </li>   
            </ul>
            
<%
          }
      }
     
	 /* ============================================================================== */
	 /* =   1. 정상 취소(재승인) 시 결과 출력 ( res_cd값이 0000인 경우)                  		     = */
	 /* = -------------------------------------------------------------------------- = */
     if ( "mod".Equals ( req_tx ))
     {
   	    if("0000".Equals ( res_cd ))
  	  {
            
%>      
            <li>
               <div class="left"><p class="title">NHN KCP 거래번호</p></div>
               <div class="right">
                   <div class="ipt-type-1 pc-wd-2">
                       <%=tno %>
                   </div>
               </div>
           </li>
<% 
         if(pnt_amount != null)
  	  {
            
%>      
            <li>
               <div class="left"><p class="title">재승인금액</p></div>
               <div class="right">
                   <div class="ipt-type-1 pc-wd-2">
                       <%=pnt_amount %>
                   </div>
               </div>
           </li>
           <li>
               <div class="left"><p class="title">승인번호</p></div>
               <div class="right">
                   <div class="ipt-type-1 pc-wd-2">
                       <%=pnt_app_no %>
                   </div>
               </div>
           </li>
        </ul>

<%          }
          }
      }
          
%> 
            <ul class="list-btn-2">
                <li class="pc-only-show"><a href="index.html" class="btn-type-3 pc-wd-2">처음으로</a></li>
            </ul>
        </div>
        <div class="grid-footer">
            <div class="inner">
                <!-- footer -->
                <div class="footer">
                    ⓒ NHN KCP Corp.
                </div>
                <!-- //footer -->
            </div>
        </div>
    </div>
    <!--//wrap-->
</body>
</html>