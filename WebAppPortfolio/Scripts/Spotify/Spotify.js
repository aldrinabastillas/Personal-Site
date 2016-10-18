angular.module('spotifyApp', [])
    .controller('SpotifyController', ['$scope', '$http', function ($scope, $http) {
        var spotify = this;

        spotify.Search = function (searchTerm) {
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