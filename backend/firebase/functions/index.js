import { onRequest } from "firebase-functions/v2/https";
import express from "express";
import OpenAI from "openai";
import dotenv from "dotenv";


dotenv.config();
const app = express();

app.get('/', async (req, res) => {

    const openai = new OpenAI({
        baseURL: "https://openrouter.ai/api/v1",
        apiKey: process.env.ORK
        // defaultHeaders: {
        //   "HTTP-Referer": "<YOUR_SITE_URL>", // Optional. Site URL for rankings on openrouter.ai.
        //   "X-Title": "<YOUR_SITE_NAME>", // Optional. Site title for rankings on openrouter.ai.
        // },
    });

    try {
        const completion = await openai.chat.completions.create({
        model: "nvidia/nemotron-nano-9b-v2:free",
        messages: [
            {
            "role": "user",
            "content": "What is the meaning of life?"
            }
        ],
        });
    res.send(completion.choices[0].message.content);
    
    } catch (error) {
        console.error(error);
        res.status(500).send("Error calling LLM");
    }
});

export const callLLM = onRequest(app);
