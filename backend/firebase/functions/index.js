import { onRequest } from "firebase-functions/v2/https";
import express from "express";
import OpenAI from "openai";
import dotenv from "dotenv";


// dotenv.config();
const app = express();

app.get('/:npc_name', async (req, res) => {

    

    const npc_data = {
        "wallet_npc":{
            "system_prompt": 
            `
            You are an NPC in a game and you are a simple citizen in the city you are in.
            You have just lost your wallet somewhere in the city, near the location of you and the player. 
            You do now know any details about where it was lost or how it was lost.
            You are concerned, stressed, worried. You seek help from the player, who has approached you.
            Your response should be short and use simple vocabulary that's easy to understand. No more than 15 words per response.
            The response format should resemble dialogue in language learning books or exercises for beginners of that language.
            Output only your dialogue. Do not include any other text that would not be said out loud.   
            `
        },
        "police_npc":{
            "system_prompt": 
            `
            You are an NPC in a game and you are a police officer in the city you are in.
            You are friendly and willing to help with any situation that the player and other citizens approach you with.
            If approached with a concern, you should ask the player to describe the situation in detail.
            Your response should be short and use simple vocabulary that's easy to understand. No more than 15 words per response.
            The response format should resemble dialogue in language learning books or exercises for beginners of that language.
            Output only your dialogue. Do not include any other text that would not be said out loud.   
            `
        },
        "bread_npc":{
            "system_prompt": 
            `
            You are an NPC in a game and you are food vendor on the street, in the city you are in. You are selling loaves of warm bread.
            You are friendly and pleased to serve any potential customers that approach you.
            Your response should be short and use simple vocabulary that's easy to understand. No more than 15 words per response.
            The response format should resemble dialogue in language learning books or exercises for beginners of that language.
            Output only your dialogue. Do not include any other text that would not be said out loud.    
            `
        }
    }


    const current_npc = npc_data[req.params.npc_name];

    const messages = [
        {
            "role": "system",
            "content": current_npc.system_prompt
        },
        {

            "role": "user",
            "content": "Are you looking for something?"
        }
    ]



    const openai = new OpenAI({
        baseURL: "https://router.huggingface.co/v1",
        apiKey: process.env.LIVE_HFTOKEN
    });

    try {
        const response = await openai.responses.create({
        model: "openai/gpt-oss-20b",
        input: messages,
        });

    res.send(response.output_text);
    
    } catch (error) {
        console.error(error);
        res.status(500).send("Error calling LLM");
    }
});

export const callLLM = onRequest(app);
