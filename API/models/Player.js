const mongoose = require("mongoose");

const playerSchema = new mongoose.Schema({
    screenName:String,
    firstName:String,
    lastName:String,
    dateStartPlaying:String,
    score:Number
})

module.exports = mongoose.model("Player", playerSchema);