local score = 0;

function scorePerPoint(point)
	score=score+point.x;
end

function getScore()
	return score;
end

function getIsEndCriteria()
	return true;
end