import { onRequest } from "firebase-functions/v2/https";
import express from "express";
import OpenAI from "openai";
import dotenv from "dotenv";
import npc_data from "./npc_data.js";

// dotenv.config();
const app = express();

app.get('/:npc_name', async (req, res) => {


    const current_npc = npc_data[req.params.npc_name];

    const messages = [
        {
            "role": "system",
            "content": current_npc.system_prompt
        },
        {

            "role": "user",
            "content": "Do you know my name?"
        }
    ]


    const openai = new OpenAI({
        baseURL: "https://api.openai.com/v1",
        apiKey: process.env.LIVE_OPENAIKEY
    });

    try {
        const response = await openai.responses.create({
        model: "gpt-5-nano",
        // previous_response_id: "<response_id here>",
        input: messages,
        store: true
        });

    res.send(response.output_text);

    
    } catch (error) {
        console.error(error);
        res.status(500).send("Error calling LLM");
    }
});

export const callLLM = onRequest(app);
