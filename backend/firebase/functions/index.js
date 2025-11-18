import { onRequest } from "firebase-functions/v2/https";
import express from "express";
import OpenAI from "openai";
import dotenv from "dotenv";
import npc_data from "./npc_data.js";

// dotenv.config();
const app = express();

app.get('/:npc_name', async (req, res) => {


    const current_npc = npc_data[req.params.npc_name];
    const system_prompt = current_npc.system_prompt;
    // const mission_prompt = current_npc.mission_prompts[req.params.mission_name];
    const mission_prompt = current_npc.mission_prompts["m_1_d_4"];
    
    
    const messages = [
        {
            "role": "system",
            "content": system_prompt
        },
        {
            "role": "developer",
            "content": mission_prompt
        },
        {

            "role": "user",
            "content": "The distance between the sun and the earth is called an Astronomical Unit"
        }
    ]


    const openai = new OpenAI({
        baseURL: "https://api.openai.com/v1",
        apiKey: process.env.LIVE_OPENAIKEY
    });

    try {
        const response = await openai.responses.create({
        model: "gpt-5-nano",
        previous_response_id: "resp_0a273c472fd42cf100691bf98159f8819789dde44f5589558f",
        input: messages,
        store: true
        });

    res.send(response.output_text + "<br>" + response.id);

    
    } catch (error) {
        console.error(error);
        res.status(500).send("Error calling LLM");
    }
});

export const callLLM = onRequest(app);
