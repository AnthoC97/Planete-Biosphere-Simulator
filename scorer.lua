populationSize = 200;
local points = getPoints();

function getScore()
	local score = 0;
	local mountains = 0;
	local plains = 0;

	for i, point in ipairs(points) do
		local elevation = noiseScript.GetNoiseGenerator().GetNoise3D(point);
        if isNaN(elevation) or isInfinity(elevation) then score=score-9999999; end
        --if elevation == 0 then score=score+1; end
		if elevation >= 0.1 then
			mountains=mountains+1;
		elseif elevation <= 0.05 then
			plains = plains+1;
		else
			score=score-1;
		end
    end

	score = score-(math.abs(plains-mountains));
	return score;
end

function isEndCriteria()
	return bestScore >= 0;
end