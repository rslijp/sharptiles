String.prototype.startsWith = function(str){ return (this.match("^" + str) == str) }

function goto(url){
    new Ajax.Updater('pageUpdate', url, { method: 'get', onComplete: updatePage });
}

function updatePage()
{
    var update = $('pageUpdate');
    $('pageUpdate').childElements().each(
        function(child)
        {
            var id = "" + child.identify();
            if (id.startsWith("update-"))
            {
                var idToUpdate = id.substr("update-".length, (id.length - "update-".length));
                $(idToUpdate).replace($(id));
                $(id).id = idToUpdate;
                $(idToUpdate).addClassName('blink');
                setTimeout("removeBlink('" + idToUpdate + "')", 1000);
                
            }

        }
    ); 
    runScripts();
}

function removeBlink(id)
{
    $(id).removeClassName('blink');
}

function runScripts()
{
    var injections = $$('.javascriptInjection');
    injections.each(
        function(injected)
        {
            eval(injected.innerHTML);
            injected.remove();
        }
    ); 
}

function postForm(id, url)
{
    var theForm = document.getElementById(id);

    var pars = Form.serialize(theForm);
    new Ajax.Updater('pageUpdate', url, { method: 'post', parameters: pars, onComplete: updatePage });
}

