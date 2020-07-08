require("state");

function execute()
    if (sharedContext["stamina"] < 100) then
        sharedContext["stamina"] = sharedContext["stamina"] + 1;
    end

    if (sharedContext["stamina"] >= 100) then
        sharedContext["currentState"] = state.idle;
        return true, true;
    end

    return false, false;
end
