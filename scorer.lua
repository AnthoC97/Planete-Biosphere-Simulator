populationSize = 200;
local points = getPoints();

function getScore()
	local score = 0;
	for i, point in ipairs(points) do
		local elevation = noiseScript.GetNoiseGenerator().GetNoise3D(point);
        if isNaN(elevation) or isInfinity(elevation) then score=score-9999999; end
        if elevation == 0 then score=score+1; end
    end
	
	--print(noiseScript.GetColor(points[1]));
	
	score = score/#points;
	return score;
end

function isEndCriteria()
	return bestScore >= 1;
end