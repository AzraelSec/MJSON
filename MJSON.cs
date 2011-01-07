using System;
using System.Text;
using System.Collections;

namespace Menagement.JSON
{          
    public static class JSON_Object
    {

#if  DEBUG
        public static void Main()
        {
			try
			{
				string s = "{\"error\":0}";
            	Hashtable t = (Hashtable)Decode(s);
            
            	if(t != null)
                	foreach(DictionaryEntry d in t)
                    	Console.WriteLine(d.Key.ToString());
            	else
                	Console.WriteLine("Doesn't work! -.-");		
			}
			catch(Exception e)
			{
					Console.WriteLine(e.Message);
			}
        }
#endif
        
        private const int JSON_TOKEN_NONE = 1;
        private const int JSON_CURLY_OPEN = 2;
        private const int JSON_CURLY_CLOSE = 3;
        private const int JSON_COLON = 4;
        private const int JSON_COMMA = 5;
        private const int JSON_STRING = 6;
        private const int JSON_NUMBER = 7;
        private const int MAX_LINE = 1000;
        
        private static void Jump_escape(char[] vett, ref int index)
        {
            for(; index < vett.Length; index++)
                if(vett[index] != '\n' ||
                   vett[index] != '\t' ||
                   vett[index] != '\r' ||
                   vett[index] != ' ')
                    break;
        }
        
        private static int NextToken(char[] vett, ref int index)
        {
            Jump_escape(vett, ref index);
            
            if(index == vett.Length)
                return JSON_Object.JSON_TOKEN_NONE;
            
            char c = vett[index];
            index++;
            
            switch(c)
            {
                case '{':
                    return JSON_Object.JSON_CURLY_OPEN;                
                case '}':
                    return JSON_Object.JSON_CURLY_CLOSE;
                case ':':
                    return JSON_Object.JSON_COLON;
                case ',':
                    return JSON_Object.JSON_COMMA;
                case '"':
                    return JSON_Object.JSON_STRING;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    return JSON_Object.JSON_NUMBER;
            }
            
            index--;
            return JSON_Object.JSON_TOKEN_NONE;
        }
        
        private static int NextToken(char[] vett, int index)
        {
   
            Jump_escape(vett, ref index);
            
            if(index == vett.Length)
                return JSON_Object.JSON_TOKEN_NONE;
            
            char c = vett[index];
            
            switch(c)
            {
                case '{':
                    return JSON_Object.JSON_CURLY_OPEN;
                case '}':
                    return JSON_Object.JSON_CURLY_CLOSE;
                case ':':
                    return JSON_Object.JSON_COLON;
                case ',':
                    return JSON_Object.JSON_COMMA;
                case '"':
                    return JSON_Object.JSON_STRING;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    return JSON_Object.JSON_NUMBER;
            }
            
            return JSON_Object.JSON_TOKEN_NONE;
        }
        
        private static string ParseString(char[] vett, ref int index, ref bool result)
        {
            StringBuilder final = new StringBuilder();
            
            Jump_escape(vett, ref index);
            
            if(index == vett.Length)
                return null;
            
            char c = vett[index++];
            bool comp = false;
            
            while(!comp)
            {
                c = vett[index++];
                
                if(c == '"')
                {
                    comp = true;
                    break;
                }
                else
                    final.Append(c);
            }
            
            if(!comp)
            {
                result = false;
                return null;
            }
            else
                return final.ToString();
        }
        
        private static Hashtable ParseObject(char[] vett, ref int index, ref bool success)
        {
            Hashtable table = new Hashtable();
            NextToken(vett, ref index);
            int token;
            
            while(1 == 1)
            {
                token = NextToken(vett, index);
                
                if(token == JSON_Object.JSON_TOKEN_NONE)
                {
                    success = false;
                    return null;
                }
                else
                    if(token == JSON_Object.JSON_COMMA)
                        NextToken(vett, ref index);
                else
                    if(token == JSON_Object.JSON_CURLY_CLOSE)
                    {
                        NextToken(vett, ref index);
                        return table;
                    }
                else
                {
                    string name = ParseString(vett, ref index, ref success);
                        
                    if(!success)
                        return null;
                        
                    token = NextToken(vett, ref index);
                        
                    if(token != JSON_Object.JSON_COLON)
					{
						success = false;
                        return null;
					}
                        
                    object final = ParseValue(vett, ref index, ref success);
                
                    if(!success)
                        return null;
                            
                    table.Add(name, final);
                }
            }
            return table;
        }
        
        private static object ParseValue(char[] vett, ref int index, ref bool success)
        {
            switch(NextToken(vett, index))
            {
                case JSON_Object.JSON_CURLY_OPEN:
                    return ParseObject(vett, ref index, ref success);
                case JSON_Object.JSON_NUMBER:
                    return ParseNumber(vett, ref index);
                case JSON_Object.JSON_STRING:
                    return ParseString(vett, ref index, ref success);
                case JSON_Object.JSON_TOKEN_NONE:
                    break;
                default:
                    success = false;
                    return null;
            }
            
            return null;
        }
        
        private static int ParseNumber(char[] vett, ref int index)
        {
            Jump_escape(vett, ref index);
            int last = LastNumber(vett, index);
            int vettLength = (last - index) + 1;
            char[] charNumber = new char[vettLength];
            Array.Copy(vett, index, charNumber, 0, vettLength);
            index = ++last;
            
            return Int32.Parse(new string(charNumber));
        }
        
        private static int LastNumber(char[] vett, int index)
        {
            for(; index < vett.Length; index++)
                if(vett[index] != '0' ||
                   vett[index] != '1' ||
                   vett[index] != '2' ||
                   vett[index] != '3' ||
                   vett[index] != '4' ||
                   vett[index] != '5' ||
                   vett[index] != '6' ||
                   vett[index] != '7' ||
                   vett[index] != '8' ||
                   vett[index] != '9' ||
                   vett[index] != '-' )
                    break;
            
            return index--;
        }
        
        public static object Decode(string json)
        {
            if(json != null)
            {
                bool success = true;
                char[] ObjectArray = json.ToCharArray();
                int index = 0;
                object final = ParseValue(ObjectArray, ref index, ref success);
                return final;
            }
            else
                return null;
        }
    }
}
