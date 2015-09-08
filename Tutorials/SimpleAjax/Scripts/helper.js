String.prototype.startsWith = function(str){ return (this.match("^" + str) == str) }

function goto(url){
    new Ajax.Updater('content', url, { method: 'get', onComplete: updatePage });
}

function updatePage()
{
    $("content").addClassName('blink');
    setTimeout("removeBlink('content')", 1000);
}

function removeBlink(id)
{
    $(id).removeClassName('blink');
}
