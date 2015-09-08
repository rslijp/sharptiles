<?php
     $mainContent = $_REQUEST['mainContent'] ;
     if(strlen($mainContent)==0) $mainContent='home'; 
     if(strpos($mainContent, "//")) $mainContent='home'; 
     $contentSplit = explode(",", $mainContent);
     $mainContent=$contentSplit[0];
     
	function isCurrent($tabName){
	   $selected = $_REQUEST['mainContent'] ;
	   $contentSplit = explode(",", $selected);

  	   if(sizeof($contentSplit)>1){
        	$selected=$contentSplit[1];    
	   } 
	   if($selected==$tabName) echo "class=\"current\"";
     }
?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
	
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" lang="en">
<head>

<meta name="Description" content="SharpTiles an .NET MVC and Monorail View engine" />
<meta name="Keywords" content="MVC,Monorail,View,Engine,SharpTiles,JSTL,Tiles,ViewEngine" />
<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
<meta name="Author" content="R.Z.Slijp - info@sharptiles.org" />
<meta name="Robots" content="index,follow" />	
<meta name="verify-v1" content="BIxVU8Ig4vmaWzyM+4rhFcZ+SpeDGvmhLVHvMj0/mlc=" >
<meta name="google-site-verification" content="gdcwndLyG9aU9l6aoDlEfNVfhm1qHNwgbiws2NpzBTI" />
<link rel="stylesheet" href="css/style.css" type="text/css" />
<script type="text/javascript" src="css/prototype.js"></script>

<?php include("titles.php"); ?>	
</head>

<body>
<!-- wrap starts here -->
<div id="wrap"> 

	<div id="header">	
			
		<h1 id="logo">Sharp<span class="blue">Tiles</span></h1>
		<h2 id="slogan">A view engine for ASP.NET MVC and Monorail</h2>	
	<!--	
		<form method="get" class="searchform" action="#">
			<p><input type="text" name="search_query" class="textbox" />
  			<input type="submit" name="search" class="button" value="Search" /></p>
		</form>
	-->	
	</div>		
		
	<div id="menu">
		<ul class="menu">
			<li <?php isCurrent('home'); ?>><a id="hometab" href="./index.php?mainContent=home">Home</a></li>
			<li <?php isCurrent('mission'); ?>><a id="missiontab" href="./index.php?mainContent=mission">Mission</a></li>
			<li <?php isCurrent('milestones'); ?>><a id="milestonestab" href="./index.php?mainContent=milestones">Milestones</a></li>
			<li <?php isCurrent('downloads'); ?>><a id="downloadstab" href="./index.php?mainContent=downloads">Downloads</a></li>
			<li <?php isCurrent('gettingstarted'); ?>><a id="gettingstartedtab" href="./index.php?mainContent=gettingstarted">Getting started</a></li>
			<li <?php isCurrent('tutorials'); ?>><a id="tutorialstab" href="./index.php?mainContent=tutorials">Tutorials</a></li>
			<li <?php isCurrent('documentation'); ?>><a id="referencetab" href="./index.php?mainContent=documentation">Documentation</a></li>			
			<li <?php isCurrent('profile'); ?>><a id="profiletab" href="./index.php?mainContent=profile">Profile</a></li>
			<li <?php isCurrent('contact'); ?>><a id="contacttab" href="./index.php?mainContent=contact">Contact</a></li>
		</ul>
	</div>
	
	<div id="wrap"> 

	<!-- content-wrap starts here -->	
		<div id="content-wrap">  
			<div id="maincontent">
				<?php
					include($mainContent.".fragment.html"); ?>
			</div>
		
			<div id="rightbar"><?php include("news.fragment.html"); ?> </div>

		</div>
		
	</div>
	
	
<!-- wrap ends here -->
</div>		

<!-- footer starts here -->		
		<div id="footer">
			<p>
			&copy; copyright 2008 <strong>Renzo Slijp</strong>&nbsp;&nbsp;  
			
			Design by: <a href="http://www.styleshout.com/">styleshout</a> | 
			Valid: <a href="http://validator.w3.org/check/referer">XHTML</a> | 
			<a href="http://jigsaw.w3.org/css-validator/check/referer">CSS</a>
			</p>		
		</div>	
<!-- footer ends here -->	

<script type="text/javascript">
var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www.");
document.write(unescape("%3Cscript src='" + gaJsHost + "google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E"));
</script>
<script type="text/javascript">
try {
var pageTracker = _gat._getTracker("UA-9538702-1");
pageTracker._trackPageview();
} catch(err) {}
</script>

</body>
</html>
