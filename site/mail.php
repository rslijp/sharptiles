<?php

/* All form fields are automatically passed to the PHP script through the array $HTTP_POST_VARS. */
$email = $HTTP_POST_VARS['email'];
$subject = $HTTP_POST_VARS['subject'];
$message = $HTTP_POST_VARS['message'];

$headers = "From: Contactform <web.sharptiles.org>\n";
$headers .= "Reply-To: $email\n";

mail("info@sharptiles.org","Contact form SharpTiles:".$subject,$message, $headers);

header("Location: http://www.sharptiles.org/"); 
?>
