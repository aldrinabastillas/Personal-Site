angular.module('spotifyApp', [])
    .controller('SpotifyController', ['$scope', '$http', function ($scope, $http) {
        var spotify = this;

        $('#predictionText').hide(); //hide prediction upon loading page

        spotify.availableYears = [];
        var currentYear = new Date().getFullYear();
        for (i = 1946; i < currentYear - 1; i++) {
            spotify.availableYears.push(i);
        };
        spotify.selectedYear = spotify.availableYears[spotify.availableYears.length - 1]; //default in the most recent year

        spotify.songSearch = $('#songSearch').search({
            apiSettings: {
                url: 'https://api.spotify.com/v1/search?q={query}&type=track',
                onResponse: function (spotifyResponse) {
                    var response = {
                        results: []
                    };

                    //iterate through results from Spotify
                    //See https://developer.spotify.com/web-api/search-item/
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
            fields: { //map results from Spotify to semantic-ui API
                results: 'results',
                title: 'title',
                description: 'description',
                image: 'image'
            },
            minCharacters: 3,
            onSelect(result, response) { //callback after song is selcted
                spotify.getPrediction(result.id);
                $scope.selectedSong = result.title;
                $scope.selectedArtist = result.description;

                var url = 'https://embed.spotify.com/?uri=spotify:track:' + result.id;
                $('#spotifyPlayer').attr('src', url); //change song in the player
                //$('#play-button').click(); //not supported
            }
        });

        spotify.getPrediction = function (songId) {
            $http.get('GetPrediction/' + songId)
                .success(function (response, status) {
                    var obj = JSON.parse(response);
                    var label = obj.Results.output1[0]['Scored Labels'];
                    var probability = obj.Results.output1[0]['Scored Probabilities'] * 100;
                    var prediction = 'likely';

                    if (label == 'False') {
                        probability = 100 - probability; //Given probability is that the label is True, so flip if False
                        prediction = 'not likely'
                    }

                    $scope.prediction = prediction;
                    $scope.probability = probability.toFixed(2) + '%';
                    $('#predictionText').show();
                })
                .error(function (response, status) {
                    $scope.label = 'Request failed';
                    $('#predictionText').show();
                });
        };

        spotify.billboardListCache = {};
        spotify.billboardSongs = [];
        spotify.getYearList = function (selectedYear) {
            if (spotify.billboardListCache[selectedYear] !== undefined) {
                spotify.billboardSongs = spotify.billboardListCache[selectedYear];
            }
            else {
                $http.get('GetYearList/' + selectedYear)
                .success(function (response, status) {
                    var songs = [];
                    $.each(response, function (index, item) {
                        songs.push({
                            Position: item.Position,
                            Song: item.Song,
                            Artist: item.Artist
                        });
                    });
                    spotify.billboardListCache[selectedYear] = songs; //add to cache
                    spotify.billboardSongs = songs; //update currently displayed list
                    //spotify.billboardSongs.push({
                    //    year: selectedYear,
                    //    songs: songs
                    //});
                });
            }     
        };

}]);


