var spotifyApp = angular.module('spotifyApp', []);
var mostRecentList = 2015;

spotifyApp.run(function () {
    $('table').tablesort(); //sets up a sortable table, see http://semantic-ui.com/collections/table.html#sortable

    $('a[role="button"]').click(function () { //flip accordion panel arrows
        if ($(this).attr('aria-expanded') == 'true') {
            $(this).children('span').attr('class', 'panelArrow glyphicon glyphicon-chevron-down');
        }
        else {
            $(this).children('span').attr('class', 'panelArrow glyphicon glyphicon-chevron-up');
        }
    });

    //$.get('PreloadLists/'); //loads the redis cache with all year lists
});

spotifyApp.controller('SpotifyController', ['$scope', '$http', function ($scope, $http) {
    var spotify = this;

    //populate year dropdown
    spotify.availableYears = [];
    var currentYear = new Date().getFullYear();
    for (i = mostRecentList; i >= 1946; i--) {
        spotify.availableYears.push(i);
    }
    spotify.selectedYear = spotify.availableYears[0]; //default in the most recent year

    //free text search songs in Spotify
    spotify.songSearch = $('#songSearch').search({
        apiSettings: {
            url: 'https://api.spotify.com/v1/search?q={query}&type=track',
            onResponse: function (spotifyResponse) {
                var response = {
                    results: []
                };

                //iterate through results from Spotify
                //See https://developer.spotify.com/web-api/search-item/ for structure
                $.each(spotifyResponse.tracks.items, function (i, track) {
                    response.results.push({
                        title: track.name,
                        description: track.artists[0].name,
                        image: track.album.images[2].url,
                        id: track.id
                    });
                });
                return response;
            }
        },
        fields: { //map results from Spotify to Semantic-UI API
            results: 'results',
            title: 'title',
            description: 'description',
            image: 'image'
        },
        minCharacters: 3,
        onSelect: function(result, response) { //callback after song is selected
            spotify.getPrediction(result.id);
            $scope.selectedSong = result.title;
            $scope.selectedArtist = result.description;

            var url = 'https://embed.spotify.com/?uri=spotify:track:' + result.id;
            $('#spotifyPlayer').attr('src', url); //change song in the player
            //$('#play-button').click(); //not supported by Spotify
        }
    });

    //gets a prediction for a song
    spotify.getPrediction = function (songId) {
        $http.get('GetPrediction/' + songId)
            .success(function (response, status) {
                $('#searchHelp').fadeOut('fast');
                if (response.startsWith("Try again:")) {
                    $scope.errorText = response;
                    $('#errorText').css('display', 'initial');
                    return;
                }

                var obj = JSON.parse(response);
                var label = obj.Results.output1[0]['Scored Labels'];
                var probability = obj.Results.output1[0]['Scored Probabilities'] * 100;
                var prediction = 'likely';

                if (label == 'False') {
                    probability = 100 - probability; //Given probability is that the label is True, so flip if False
                    prediction = 'not likely';
                }

                $scope.prediction = prediction;
                $scope.probability = probability.toFixed(2) + '%';
                $('#predictionText').css('visibility', 'visible');
            })
            .error(function (response, status) {
                $scope.errorText = response;
                $('#errorText').css('visibility', 'visible');
            });
    };

    //get Billboard Top 100 songs from a past year
    spotify.billboardListCache = {}; //save lists that were already loaded
    spotify.billboardSongs = [];
    spotify.getYearList = function (selectedYear) {
        //first check local cache
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
                spotify.billboardSongs = null;
                spotify.billboardSongs = songs;
            });
        }
    };

    //invoke getYearList upon page load
    spotify.getYearList(spotify.selectedYear);

    //TODO: Implement an error handler
    //$('#spotifyPlayer').error(function () {
    //    $('#spotifyPlayer').append('<p>Spotify Web Player unable to load</p>');
    //});

}]);