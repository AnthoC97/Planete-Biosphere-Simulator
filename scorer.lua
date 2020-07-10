populationSize = 200;
local points = getPoints();
local plateau = 0;
local lastBestScore = -999999999999999999;

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
	if bestScore > lastBestScore then
		lastBestScore = bestScore;
		plateau = 0;
	else
		plateau = plateau+1;
	end
	return bestScore >= 0 or plateau >= 6;
end