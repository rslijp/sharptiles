<title>SharpTiles View Engine for MVC and Monorail | 
<?php
     $titleContent = $_REQUEST['mainContent'] ;
     if(strlen($titleContent)==0) $titleContent='home';
     $titleContent = str_replace(",","-", $titleContent);   
     $titleMap = array(
		"home"=>"Fast, Testable and Clean.",                   
		"gettingstarted"=>"Getting started",                  
		"profile"=>"Profile",                   
		"downloads"=>"Downloads",                   
		"milestones"=>"Milestones",                   
		"documentation"=>"Documentation",                   
		"contact"=>"Contact",                   
		"tagreference-documentation"=>"Documentation | Tag reference",                   
		"tiles.config.dtd-documentation"=>"Documentation | Dtd for tiles config",                   
		"tilesguide-documentation"=>"Documentation | Guide for tiles usage",                   
		"mission"=>"Mission",
		"tutorials"=>"Tutorials",
		"plaintiles-tutorials" => "Tutorials | Plain tiles",
		"cleantiles-tutorials" =>  "Tutorials | Cleaner tiles ",
		"nonembedded-tutorials" => "Tutorials | Non embedded tutorials",
		"bundles-tutorials" => "Tutorials | Resouce bundles",
		"actionsandurls-tutorials" => "Tutorials | Actions and urls",
		"extendingthetaglib-tutorials" => "Tutorials | Extending the tag lib",
		"simpleajax-tutorials" => "Tutorials | Simple ajax",
		"customrepository-tutorials" => "Tutorials | Custom tile repository",
		"multiviewengine-tutorials" => "Tutorials | Multi view engine"); 
     
     echo $titleMap[$titleContent];
?></title>
