import { onRequest } from "firebase-functions/v2/https";
import express from "express";
import OpenAI from "openai";
import dotenv from "dotenv";
import npcData from "./npcData.js";

//for local emulation only
dotenv.config();

const app = express();
app.use(express.json());

const openai = new OpenAI({
    baseURL: "https://api.openai.com/v1",
    apiKey: process.env.LIVE_OPENAIKEY
});


const callDialogueLLM = async (npcName, dialogueIndex = 0, userInput = "", prevRespID = undefined) => {
    
    const currentNPC = npcData[npcName];
    const systemPrompt = currentNPC.systemPrompt;
    const dialogueName = currentNPC.dialogueSequence[dialogueIndex];
    const currentDialogue = currentNPC.missionPrompts[dialogueName];
    
    const messages = [
        {
            "role": "system",
            "content": systemPrompt
        },
        {
            "role": "developer",
            "content": currentDialogue
        },
        {
            "role": "user",
            "content": userInput,
        }
    ]

    const response = await openai.responses.create({
        model: "gpt-5-nano",
        ...(prevRespID && { previous_response_id: prevRespID }),
        // previous_response_id: "resp_0a273c472fd42cf100691bf98159f8819789dde44f5589558f",
        input: messages,
        store: true
    });

    return {
            outputText: response.output_text, 
            responseID: response.id
    };
};


app.get('/dialogue/:npcName', async (req, res) => {
//needs to be changed to post and req.body
    
    try {

        const { npcName, dialogueIndex, userInput, prevRespID } = req.params;
        
        const dialogueResponse = await callDialogueLLM(npcName, dialogueIndex, userInput, prevRespID);
        res.send(dialogueResponse);

    } catch (error) {
        
        console.error(error);
        res.status(500).send("Error calling LLM");
    
    }    
});



const semanticEval = async (npcName, dialogueIndex = 0, userInput = "", prevRespID = undefined) => {

    const currentNPC = npcData[npcName];
    const dialogueName = currentNPC.dialogueSequence[dialogueIndex];
    const correctResponse = currentNPC.correctResponses[dialogueName];

    const messages = [
        {
            role: "system",
            content: `
            You are a dialogue evaluator for player inputs that are in response to NPC outputs.
            You can see the recent conversation between the NPC and the player (user).
            Use that context plus the extra information in this message to judge the latest player reply.
            Do not rewrite and do not correct the user input or user responses.
            You must be strict in regard to grammar and spelling errors.
            You will be given the conversaton history, a correct reference phrase (Correct Reference Response), and a response from the user (User Input). 
            `,
        },
        {
            role: "user",
            content: `

            Correct Reference Response: "${correctResponse}"
            User Input: "${userInput}"

            Is the user input phrase free of spelling errors?
            If so, is the user input free of language syntax errors?
            If so, does the user input phrase make sense as a reply to the LLM NPCs last output and is similar to the correct reference response?
            Return a strict JSON object with: {"passed": "yes"|"no", "reason": "<short explanation>"}
            
            `,
        },
    ]

    const response = await openai.responses.create({
        model: "gpt-5-nano",
        ...(prevRespID && { previous_response_id: prevRespID }),
        input: messages,
        store: true,
    });

    const outputText = response.output_text.trim();
    
    try {
        const parsedOutput = JSON.parse(outputText);
        return parsedOutput;
    } catch(error) {
        console.log("JSON parse failed, extracting text...");

        const extractedText = outputText.toLowerCase().substring(0, 5);

        let passed = "no";

        if (extractedText.includes("yes")) {
            passed = "yes";
        }
    
        return {
            passed: passed,
            reason: outputText
        }

    }

};

app.get('/evaluate', async (req, res) => {
    try {

        //needs to be changed to post and req.body
        const { npcName, dialogueIndex, userInput, prevRespID } = req.params;
        
        const evalResponse = await semanticEval(npcName, dialogueIndex, userInput, prevRespID);
        res.send(evalResponse);

    } catch (error) {
        console.error(error);
        res.status(500).send("Error calling LLM");
    }
});

export const callLLM = onRequest(app);
