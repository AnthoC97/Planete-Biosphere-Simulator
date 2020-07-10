require("state");

function execute()
    local editableContext = sharedContext.GetEditable();

    if (editableContext["stamina"] < 100) then
        editableContext["stamina"] = editableContext["stamina"] + 1;
    end

    if (editableContext["stamina"] >= 100) then
        editableContext["currentState"] = state.idle;
        sharedContext.Synchronize(editableContext);
        return true, true;
    end

    sharedContext.Synchronize(editableContext);
    return false, false;
end
