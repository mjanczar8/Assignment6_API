const express = require("express")
const path = require("path")

const mongoose = require("mongoose")
const bodyParser = require("body-parser")

const { nanoid } = require("nanoid");

const app = express();
const port = 3000

const Player = require("./models/Player")

app.use(bodyParser.json());
app.use(express.urlencoded({extended:true}));
app.use(express.static(path.join(__dirname, "public")))

const mongoURI = "mongodb://localhost:27017/Monogame";
mongoose.connect(mongoURI);

const db = mongoose.connection;

db.on("error", console.error.bind(console, "MongoDB connection error"));
db.once("open", ()=>{
    console.log("Connected to MongoDB Database");
});

app.get("/player", async (req, res)=>{
    try{
        const players = await Player.find();
        res.json(players);
        console.log(players);
    }catch(err){
        console.log("Failed to get players.");
    }
});

app.get("/player/:screenName", async (req, res) => {
    try {
        console.log("Searching for player:", req.params.screenName); // Debug Log

        const player = await Player.findOne({ screenName: req.params.screenName });

        if (!player) {
            console.log("Player not found in database."); // Debug Log
            return res.status(404).json({ error: "Player not found" });
        }

        console.log("Player found:", player); // Debug Log
        res.json(player);
        
    } catch (error) {
        console.error("Error retrieving player by name:", error);
        res.status(500).json({ error: "Failed to retrieve player" });
    }
});

app.post("/sentdata", (req,res)=>{
    const newPlayerData = req.body;

    console.log(JSON.stringify(newPlayerData,null,2));

    res.json({message:"Player Data recieved"});
});

app.delete("/delete/:screenName", async (req,res)=>{
    try{
        const playername = req.query;
        const player = await Player.find(playername);

        if(player.length === 0){
            return res.status(404).json({error:"Failed to find the person."});
        }

        const deletedPlayer = await Player.findOneAndDelete(playername);
        res.json({message:"Person deleted Successfully"});

    }catch(err){
        console.log(err);
        res.status(404).json({error:"Person not found"});
    }
});

app.post("/sentdatatodb", async (req,res)=>{
    try{
        const newPlayerData = req.body;

        console.log(JSON.stringify(newPlayerData,null,2));

        const newPlayer = new Player({
            playerid:nanoid(8),
            screenName:newPlayerData.screenName,
            firstName:newPlayerData.firstName,
            lastName:newPlayerData.lastName,
            dateStartedPlaying:newPlayerData.dateStartedPlaying,
            score:newPlayerData.score

        });
        //save to database
        await newPlayer.save();
        res.json({message:"Player Added Successfully",playerid:newPlayer.playerid, name:newPlayer.screenNameName});
    }
    catch(error){
        res.status(500).json({error:"Failed to add player"})
    }
    
    
});

//Update Player
app.put("/update/:screenName", async (req, res) => {
    try {
        const { screenName } = req.params;
        const updatedData = req.body;

        console.log(`Updating player: ${screenName}`);

        const updatedPlayer = await Player.findOneAndUpdate(
            { screenName },
            { $set: updatedData },
            { new: true } // Returns updated document
        );

        if (!updatedPlayer) {
            console.log("Player not found.");
            return res.status(404).json({ error: "Player not found" });
        }

        console.log("Player updated:", updatedPlayer);
        res.json({ message: "Player updated successfully", updatedPlayer });
    } catch (error) {
        console.error("Error updating player:", error);
        res.status(500).json({ error: "Failed to update player" });
    }
});



app.listen(3000, ()=>{
    console.log("Running on port 3000");
})