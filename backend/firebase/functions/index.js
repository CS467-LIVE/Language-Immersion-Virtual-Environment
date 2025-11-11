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
