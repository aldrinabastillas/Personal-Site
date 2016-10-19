angular.module('spotifyApp', [])
    .controller('SpotifyController', ['$scope', '$http', function ($scope, $http) {
        var spotify = this;


        spotify.songSearch = $('#songSearch').search({
            apiSettings: {
                url: 'https://api.spotify.com/v1/search?q={query}&type=track',
                onResponse: function (spotifyResponse) {
                    var response = {
                        results: []
                    };

                    // translate GitHub API response to work with search
                    $.each(spotifyResponse.tracks.items, function (i, track) {
                        response.results.push({
                            title: track.name,
                            description: track.artists[0].name,
                            image: track.album.images[2].url,
                            id: track.id
                        });
                    });
                    return response;
                },
            },
            fields: {
                results: 'results',
                title: 'title',
                description: 'description',
                image: 'image'
            },
            minCharacters: 3,
            onSelect(result, response) {
                spotify.IdSearch(result.id);
            }
        });

        spotify.IdSearch = function (searchTerm) {
            var id = searchTerm;

            $http.get('GetPrediction/' + id)
                .success(function (response, status) {
                    var obj = JSON.parse(response);
                    var label = obj.Results.output1[0]['Scored Labels'];
                    var probability = obj.Results.output1[0]["Scored Probabilities"] * 100;
                    if (label == 'False') {
                        probability = 100 - probability; //Given probability is that the label is True, so flip if False
                    }

                    $scope.label = 'Prediction: ' + label;
                    $scope.probability = 'Probability: ' + probability.toFixed(2) + '%';
                })
                .error(function (response, status) {
                    $scope.label = 'Request failed';
                });
        };
}]);


