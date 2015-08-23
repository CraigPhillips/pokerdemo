$(document).ready(function () {
    // Sets up drop-downs to modify selected card appearances.
    $(".rank-selector, .suit-selector").on("change", updateCardAppearance);

    // Sets up automatic hand dealing button.
    $("#deal-hands-button").on("click", dealHands);

    // Overrides default submit behavior to perform validation first
    $("#hands input[type=\"submit\"]").on("click", validateAndSubmitPokerInput);

    overrideBackLinks();

    // Triggers change events for each rank selector so that if a user hits the 
    // back button, card images are displayed correctly.
    $(".rank-selector").trigger("change");
});

/* 
    Inspects the paired suit & rank selector and then assigns the appropriate class
    to the proper card so that it's appearance can change to reflect the correct
    card.
*/
function updateCardAppearance() {
    if ($(this).length) {
        var cardWrapper = $(this).parents(".card-wrapper");
        if (cardWrapper.length) {
            var selectedSuit = cardWrapper.find(".suit-selector").val();
            var selectedRank = cardWrapper.find(".rank-selector").val();
            var card = cardWrapper.find(".card");

            // Clears all classes so that the correct new ones will be the only
            // ones in place.
            card.removeClass();
            card.addClass("card");
            if (selectedSuit && selectedRank) {
                card.addClass("suit-" + selectedSuit.toLowerCase());
                card.addClass("rank-" + selectedRank.toLowerCase());
            }
        }
    }
}

/*
    Validates that all input values are valid and then triggers the form's submit.
*/
function validateAndSubmitPokerInput(eventData) {
    eventData.preventDefault();
    // Clears any old errors.
    $(".error-message").hide();

    var errorEncountered = false;
    $(".player-name").each(function () {
        if (!$(this).val() || $(this).val().trim().length == 0) {
            errorEncountered = true;
            $("#missing-name-error").show();
        }
    });

    if (!errorEncountered) {
        var cardDescriptions = [];
        $(".card").each(function () {
            // This will look something like "card suit-hearts rank-nine" and should
            // uniquely identify the card on the page.
            var currentCardDescription = $(this).attr("class");

            if (!currentCardDescription || currentCardDescription.split(" ").length != 3) {
                // Card value has not been set
                errorEncountered = true;
                $("#missing-card-error").show();

                // Breaks out of loop.
                return false;
            }
            else if (cardDescriptions.indexOf(currentCardDescription) != -1) {
                // Card value has been used more than once.
                errorEncountered = true;
                $("#duplicate-card-error").show();

                // Breaks out of loop.
                return false;
            }
            else cardDescriptions.push(currentCardDescription);
        });
    }

    if (!errorEncountered) {
        $("#hands").submit();
    }
}

/*
    Retrieves and then populates hands from the server.
*/
function dealHands() {
    // Clears any old errors.
    $(".error-message").hide();

    $.ajax("cardhands")
        // Hands generated, so populates UI elements.
        .done(function (handsData) {
            if (!handsData || !handsData || !handsData.requestedHands || !handsData.requestedHands.length) 
                handleHandDealingError();
            else {
                for (var handsDealt = 0; handsDealt < handsData.requestedHands.length; handsDealt++) {
                    var hand = handsData.requestedHands[handsDealt];

                    if (!hand.length) {
                        handleHandDealingError();
                        break;
                    }
                    else {
                        for (var cardsDealt = 0; cardsDealt < hand.length; cardsDealt++) {
                            var card = hand[cardsDealt];

                            if (!card.rank || !card.suit) {
                                handleHandDealingError();
                                break;
                            }
                            else {
                                var targetRankSelector =
                                    $(
                                        "[name=\"rank-player-" +
                                        (handsDealt + 1).toString() + "-" +
                                        "card-" +
                                        (cardsDealt + 1).toString() + "\"]");
                                var targetSuitSelector =
                                    $(
                                        "[name=\"suit-player-" +
                                        (handsDealt + 1).toString() + "-" +
                                        "card-" +
                                        (cardsDealt + 1).toString() + "\"]");

                                if (!targetRankSelector.length || !targetSuitSelector.length) {
                                    handDealingError();
                                    break;
                                }
                                else {
                                    targetRankSelector.val(card.rank);
                                    targetSuitSelector.val(card.suit);

                                    // Manually triggers change event to update the UI.
                                    targetSuitSelector.trigger("change");
                                }
                            }
                        }
                    }
                }
            }
        })
        // Error occurred during retrieval of hands, so simply displays an error message.
        .fail(handleHandDealingError);
}

/*
    Displays error message if something goes wrong while attempting to automatically deal
    hands.
*/
function handleHandDealingError() { 
    $("#hand-dealing-error").show();
}

/*
    Prevents any links marked as back-links to trigger the browser's "back" button rather
    than go to the acutal link's destination.
*/
function overrideBackLinks() {
    if (history && history.back) {
        $("a.back-link").on("click", function (eventData) {
            eventData.preventDefault();
            history.back();
        })
    }
}